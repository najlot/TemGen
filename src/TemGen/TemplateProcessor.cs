using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemGen.Handler;

namespace TemGen;

public class TemplateProcessor
{
	private readonly Project _project;
	private readonly List<Definition> _definitions;

	private readonly AbstractSectionHandler _handler;

	public TemplateProcessor(Project project, List<Definition> definitions)
	{
		_project = project;
		_definitions = definitions;

		_handler = new TextSectionHandler()
			.SetNext(new CsSectionHandler())
			.SetNext(new ReflectionSectionHandler())
			.SetNext(new PySectionHandler())
			.SetNext(new JintSectionHandler())
			.SetNext(new LuaSectionHandler())
			;
	}

	public async Task<Dictionary<string, string>> Handle(Template template, List<Definition> definitions)
	{
		Dictionary<string, string> results = new();

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
			Definitions = _definitions,
			DefinitionEntry = definitionEntry,
			Entries = definition.Entries,
			SkipOtherDefinitions = false,
			Project = _project,
			RepeatForEachDefinitionEntry = false
		};

		foreach (var section in template.Sections)
		{
			try
			{
				await _handler.Handle(globals, section).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				throw new Exception($"Error processing: [{section.Handler}]{Environment.NewLine}{section.Content}{Environment.NewLine}", ex);
			}
		}

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
}