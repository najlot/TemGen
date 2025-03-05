using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemGen.Handler;

namespace TemGen;

public class TemplateProcessor(AbstractSectionHandler[] handler, Project project, List<Definition> definitions)
{
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

		foreach (var section in template.Sections)
		{
			try
			{
				foreach (var handler in handler)
				{
					if (await handler.TryHandle(globals, section).ConfigureAwait(false))
					{
						break;
					}
				}
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