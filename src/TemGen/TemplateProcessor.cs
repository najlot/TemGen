using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TemGen.Handler;

namespace TemGen;

public class TemplateProcessor(AbstractSectionHandler[] handler, Project project, List<Definition> definitions)
{
	private static readonly Regex ForPattern = new(@"^\s*(?<name>[A-Za-z_][A-Za-z0-9_]*)\s+in\s+(?<expression>[\s\S]+?)\s*$", RegexOptions.Compiled);
	private readonly CsSectionHandler _csHandler = handler.OfType<CsSectionHandler>().FirstOrDefault() ?? throw new ArgumentException("TemplateProcessor requires a CsSectionHandler for C# control-flow blocks.", nameof(handler));

	public async Task<Dictionary<string, string>> Handle(Template template, List<Definition> definitions)
	{
		Dictionary<string, string> results = [];

		foreach (var definition in definitions)
		{
			var result = await Handle(template, definition, null).ConfigureAwait(false);

			if (!string.IsNullOrWhiteSpace(result.RelativePath))
			{
				results[result.RelativePath] = result.Content;
			}

			if (result.SkipOtherDefinitions)
			{
				break;
			}

			if (result.RepeatForEachDefinitionEntry)
			{
				foreach (var entry in definition.Entries)
				{
					result = await Handle(template, definition, entry).ConfigureAwait(false);

					if (!string.IsNullOrWhiteSpace(result.RelativePath))
					{
						results[result.RelativePath] = result.Content;
					}
				}
			}
		}

		return results;
	}

	public async Task<HandlingResult> Handle(Template template, Definition definition, DefinitionEntry definitionEntry)
	{
		var globals = new Globals()
		{
			RelativePath = template.RelativePath,
			Definition = definition,
			Definitions = definitions,
			DefinitionEntry = definitionEntry,
			Entries = definition.Entries,
			SkipOtherDefinitions = false,
			Project = project,
			RepeatForEachDefinitionEntry = false
		};

		await HandleSections(template.Sections, globals).ConfigureAwait(false);

		globals.ReplaceInResult("Entrys", "Entries");
		globals.ReplaceInResult("Statuss", "Status");

		return new HandlingResult
		{
			RelativePath = globals.RelativePath,
			Content = globals.Result,
			SkipOtherDefinitions = globals.SkipOtherDefinitions,
			RepeatForEachDefinitionEntry = globals.RepeatForEachDefinitionEntry
		};
	}

	private async Task HandleSections(IEnumerable<TemplateSection> sections, Globals globals)
	{
		foreach (var section in sections)
		{
			try
			{
				switch (section.Handler)
				{
					case TemplateHandler.CSharpIf:
					case TemplateHandler.CSharpElseIf:
						await HandleIfSection(section, globals).ConfigureAwait(false);
						break;

					case TemplateHandler.CSharpFor:
						await HandleForSection(section, globals).ConfigureAwait(false);
						break;

					case TemplateHandler.CSharpElse:
					case TemplateHandler.CSharpEnd:
						throw new Exception($"Unexpected control-flow token {section.Handler} during processing.");

					default:
						foreach (var currentHandler in handler)
						{
							if (await currentHandler.TryHandle(globals, section).ConfigureAwait(false))
							{
								break;
							}
						}
						break;
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Error processing: [{section.Handler}]{Environment.NewLine}{section.Content}{Environment.NewLine}", ex);
			}
		}
	}

	private async Task HandleIfSection(TemplateSection section, Globals globals)
	{
		var branch = await EvaluateCondition(section.Content, globals).ConfigureAwait(false)
			? section.Sections
			: section.ElseSections;

		await HandleSections(branch, globals).ConfigureAwait(false);
	}

	private async Task HandleForSection(TemplateSection section, Globals globals)
	{
		var (variableName, expression) = ParseForHeader(section.Content);
		var items = await EvaluateEnumerable(expression, globals).ConfigureAwait(false);

		foreach (var item in items)
		{
			globals.PushVariableScope();
			globals.SetVariable(variableName, item);

			try
			{
				await HandleSections(section.Sections, globals).ConfigureAwait(false);
			}
			finally
			{
				globals.PopVariableScope();
			}
		}
	}

	private async Task<bool> EvaluateCondition(string expression, Globals globals)
	{
		var result = await _csHandler.EvaluateExpression(globals, expression).ConfigureAwait(false);

		return result switch
		{
			bool value => value,
			null => false,
			_ => throw new Exception("The <#csif#> expression must evaluate to a boolean value.")
		};
	}

	private async Task<IEnumerable> EvaluateEnumerable(string expression, Globals globals)
	{
		expression = NormalizeLoopExpression(expression);
		var result = await _csHandler.EvaluateExpression(globals, expression).ConfigureAwait(false);

		return result as IEnumerable ?? throw new Exception("The <#csfor#> expression must evaluate to an enumerable value.");
	}

	private static (string VariableName, string Expression) ParseForHeader(string content)
	{
		var match = ForPattern.Match(content);

		if (!match.Success)
		{
			throw new Exception("Invalid <#csfor#> syntax. Use '<#csfor item in items#>'.");
		}

		return (match.Groups["name"].Value, match.Groups["expression"].Value);
	}

	private static string NormalizeLoopExpression(string expression)
	{
		var trimmed = expression.Trim();

		if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
		{
			return $"new object[] {{ {trimmed[1..^1]} }}";
		}

		return expression;
	}
}