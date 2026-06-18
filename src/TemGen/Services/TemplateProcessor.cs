using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TemGen.Handler;
using TemGen.Models;
using TemGen.Services;

namespace TemGen;

public partial class TemplateProcessor(AbstractSectionHandler[] handler, Project project, List<Definition> definitions)
{
	private static readonly Regex _forPattern = GetForPatternRegex();
	private readonly CsSectionHandler _csHandler = handler.OfType<CsSectionHandler>().FirstOrDefault() ?? throw new ArgumentException("TemplateProcessor requires a CsSectionHandler for C# control-flow blocks.", nameof(handler));
	private readonly JintSectionHandler _jsHandler = handler.OfType<JintSectionHandler>().FirstOrDefault();
	private readonly PySectionHandler _pyHandler = handler.OfType<PySectionHandler>().FirstOrDefault();
	private readonly LuaSectionHandler _luaHandler = handler.OfType<LuaSectionHandler>().FirstOrDefault();

	internal async Task<(Dictionary<string, (Encoding Encoding, string Content, bool AllowOverwrite)> Files, Dictionary<string, string> LayerContents)> HandleLayer(Template template, IReadOnlyDictionary<string, string> previousLayerContents = null)
	{
		Dictionary<string, (Encoding Encoding, string Content, bool AllowOverwrite)> results = [];
		Dictionary<string, string> layerContents = new(StringComparer.Ordinal);

		foreach (var definition in definitions)
		{
			var result = await Handle(
				template,
				definition,
				null,
				GetPreviousContent(previousLayerContents, template.RelativePath, definition, null)).ConfigureAwait(false);

			layerContents[CreatePreviousContentKey(template.RelativePath, definition, null)] = result.Content;

			if (!string.IsNullOrWhiteSpace(result.RelativePath))
			{
				layerContents[CreatePreviousContentPathKey(result.RelativePath)] = result.Content;
			}

			if (!string.IsNullOrWhiteSpace(result.RelativePath))
			{
				results[result.RelativePath] = (result.Encoding, result.Content, result.AllowOverwrite);
			}

			if (result.SkipOtherDefinitions)
			{
				break;
			}

			if (result.RepeatForEachDefinitionEntry)
			{
				foreach (var entry in definition.Entries)
				{
					result = await Handle(
						template,
						definition,
						entry,
						GetPreviousContent(previousLayerContents, template.RelativePath, definition, entry)).ConfigureAwait(false);

					layerContents[CreatePreviousContentKey(template.RelativePath, definition, entry)] = result.Content;

					if (!string.IsNullOrWhiteSpace(result.RelativePath))
					{
						layerContents[CreatePreviousContentPathKey(result.RelativePath)] = result.Content;
					}

					if (!string.IsNullOrWhiteSpace(result.RelativePath))
					{
						results[result.RelativePath] = (result.Encoding, result.Content, result.AllowOverwrite);
					}
				}
			}
		}

		return (results, layerContents);
	}

	public async Task<HandlingResult> Handle(Template template, Definition definition, DefinitionEntry definitionEntry, string previousContent = "")
	{
		var globals = new Globals()
		{
			RelativePath = template.RelativePath,
			Definition = definition,
			Definitions = definitions,
			DefinitionEntry = definitionEntry,
			Entries = definition.Entries,
			SkipOtherDefinitions = false,
			SkipRemainingRequested = false,
			Project = project,
			PreviousContent = previousContent ?? string.Empty,
			RepeatForEachDefinitionEntry = false,
			Encoding = template.Encoding,
			AllowOverwrite = true
		};

		await HandleSections(template.Sections, globals).ConfigureAwait(false);

		globals.ReplaceInResult("Entrys", "Entries");
		globals.ReplaceInResult("Statuss", "Status");

		return new HandlingResult
		{
			RelativePath = globals.RelativePath,
			Content = globals.Result,
			SkipOtherDefinitions = globals.SkipOtherDefinitions,
			RepeatForEachDefinitionEntry = globals.RepeatForEachDefinitionEntry,
			Encoding = globals.Encoding,
			AllowOverwrite = globals.AllowOverwrite
		};
	}

	private static string GetPreviousContent(IReadOnlyDictionary<string, string> previousLayerContents, string templateRelativePath, Definition definition, DefinitionEntry definitionEntry)
	{
		if (previousLayerContents is null)
		{
			return string.Empty;
		}

		return previousLayerContents.TryGetValue(CreatePreviousContentKey(templateRelativePath, definition, definitionEntry), out var previousContent)
			? previousContent
			: previousLayerContents.TryGetValue(CreatePreviousContentPathKey(templateRelativePath), out previousContent)
				? previousContent
				: string.Empty;
	}

	private static string CreatePreviousContentKey(string templateRelativePath, Definition definition, DefinitionEntry definitionEntry)
	{
		return string.Join(
			'\u001F',
			"context",
			templateRelativePath ?? string.Empty,
			definition?.Name ?? string.Empty,
			definitionEntry is null ? "definition" : "entry",
			definitionEntry?.Field ?? string.Empty,
			definitionEntry?.EntryType ?? string.Empty);
	}

	private static string CreatePreviousContentPathKey(string relativePath)
	{
		return string.Join(
			'\u001F',
			"path",
			relativePath ?? string.Empty);
	}

	private async Task HandleSections(IEnumerable<TemplateSection> sections, Globals globals)
	{
		foreach (var section in sections)
		{
			try
			{
				switch (section.Handler)
				{
					case var _ when IsConditionalHandler(section.Handler):
						await HandleIfSection(section, globals).ConfigureAwait(false);
						break;

					case var _ when IsForHandler(section.Handler):
						await HandleForSection(section, globals).ConfigureAwait(false);
						break;

						case TemplateHandler.Else:
						case TemplateHandler.End:
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

			if (globals.SkipRemainingRequested)
			{
				break;
			}
		}
	}

	private async Task HandleIfSection(TemplateSection section, Globals globals)
	{
		var branch = await EvaluateCondition(section, globals).ConfigureAwait(false)
			? section.Sections
			: section.ElseSections;

		await HandleSections(branch, globals).ConfigureAwait(false);
	}

	private async Task HandleForSection(TemplateSection section, Globals globals)
	{
		var (variableName, expression) = ParseForHeader(section);
		var items = await EvaluateEnumerable(section, expression, globals).ConfigureAwait(false);

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

			if (globals.SkipRemainingRequested)
			{
				break;
			}
		}
	}

	private async Task<bool> EvaluateCondition(TemplateSection section, Globals globals)
	{
		var result = await EvaluateExpression(section, section.Content, globals).ConfigureAwait(false);

		return result switch
		{
			bool value => value,
			null => false,
			_ => throw new Exception($"The <#{GetTagName(section.Handler, section.Language)}#> expression must evaluate to a boolean value.")
		};
	}

	private async Task<IEnumerable> EvaluateEnumerable(TemplateSection section, string expression, Globals globals)
	{
		expression = NormalizeLoopExpression(section, expression);
		var result = await EvaluateExpression(section, expression, globals).ConfigureAwait(false);

		return result as IEnumerable ?? throw new Exception($"The <#{GetTagName(section.Handler, section.Language)}#> expression must evaluate to an enumerable value.");
	}

	private Task<object> EvaluateExpression(TemplateSection section, string expression, Globals globals)
	{
		return section.Language switch
		{
			TemplateLanguage.CSharp => _csHandler.EvaluateExpression(globals, expression),
			TemplateLanguage.JavaScript => (_jsHandler ?? throw CreateMissingHandlerException(TemplateLanguage.JavaScript, section)).EvaluateExpression(globals, expression),
			TemplateLanguage.Python => (_pyHandler ?? throw CreateMissingHandlerException(TemplateLanguage.Python, section)).EvaluateExpression(globals, expression),
			TemplateLanguage.Lua => (_luaHandler ?? throw CreateMissingHandlerException(TemplateLanguage.Lua, section)).EvaluateExpression(globals, expression),
			null => throw new ArgumentException("Control-flow sections must specify a template language.", nameof(section)),
			_ => throw new ArgumentOutOfRangeException(nameof(section), section.Language, "Unsupported control-flow language.")
		};
	}

	private static (string VariableName, string Expression) ParseForHeader(TemplateSection section)
	{
		var match = _forPattern.Match(section.Content);

		if (!match.Success)
		{
			throw new Exception($"Invalid <#{GetTagName(section.Handler, section.Language)}#> syntax. Use '<#{GetTagName(section.Handler, section.Language)} item in items#>'.");
		}

		return (match.Groups["name"].Value, match.Groups["expression"].Value);
	}

	private static string NormalizeLoopExpression(TemplateSection section, string expression)
	{
		if (section is not { Handler: TemplateHandler.For, Language: TemplateLanguage.CSharp })
		{
			return expression;
		}

		var trimmed = expression.Trim();

		if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
		{
			return $"new object[] {{ {trimmed[1..^1]} }}";
		}

		return expression;
	}

	private static bool IsConditionalHandler(TemplateHandler handler)
	{
		return handler is TemplateHandler.If or TemplateHandler.ElseIf;
	}

	private static bool IsForHandler(TemplateHandler handler)
	{
		return handler == TemplateHandler.For;
	}

	private static string GetTagName(TemplateHandler handler, TemplateLanguage? language = null)
	{
		return handler switch
		{
			TemplateHandler.Script => GetLanguageTagPrefix(language),
			TemplateHandler.For => $"{GetLanguageTagPrefix(language)}for",
			TemplateHandler.If => $"{GetLanguageTagPrefix(language)}if",
			TemplateHandler.ElseIf => $"{GetLanguageTagPrefix(language)}elseif",
			TemplateHandler.Else => "else",
			TemplateHandler.End => "end",
			TemplateHandler.Reflection => "ref",
			_ => handler.ToString()
		};
	}

	private static string GetLanguageTagPrefix(TemplateLanguage? language)
	{
		return language switch
		{
			TemplateLanguage.CSharp => "cs",
			TemplateLanguage.JavaScript => "js",
			TemplateLanguage.Python => "py",
			TemplateLanguage.Lua => "lua",
			null => string.Empty,
			_ => throw new ArgumentOutOfRangeException(nameof(language), language, "Unsupported template language.")
		};
	}

	private static Exception CreateMissingHandlerException(TemplateLanguage language, TemplateSection section)
	{
		return new InvalidOperationException($"TemplateProcessor requires a {language} section handler to evaluate <#{GetTagName(section.Handler, section.Language)}#> blocks.");
	}

	[GeneratedRegex(@"^\s*(?<name>[A-Za-z_][A-Za-z0-9_]*)\s+in\s+(?<expression>[\s\S]+?)\s*$", RegexOptions.Compiled)]
	private static partial Regex GetForPatternRegex();
}