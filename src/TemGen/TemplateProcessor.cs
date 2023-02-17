using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TemGen.Handler;

namespace TemGen;

public class TemplateProcessor
{
	private readonly AbstractSectionHandler _handler;

	public TemplateProcessor(Project project, List<Definition> definitions)
	{
		_handler = new TextSectionHandler()
			.SetNext(new CsSectionHandler(project, definitions))
			.SetNext(new ReflectionSectionHandler(project, definitions))
			.SetNext(new PySectionHandler(project, definitions))
			.SetNext(new JintSectionHandler(project, definitions))
			.SetNext(new LuaSectionHandler(project, definitions))
			;
	}

	public async Task<HandlingResult> Handle(Template template, Definition definition, DefinitionEntry definitionEntry)
	{
		bool skipOtherDefinitions = false;
		bool repeatForEachDefinitionEntry = false;

		var sb = new StringBuilder();
		var result = new HandlingResult()
		{
			RelativePath = template.RelativePath,
		};

		foreach (var section in template.Sections)
		{
			try
			{
				result = await _handler.Handle(section, definition, result.RelativePath, definitionEntry).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				throw new Exception($"Error processing: [{section.Handler}]\r\n\r\n{section.Content}\r\n\r\n", ex);
			}

			sb.Append(result.Content);

			if (result.SkipOtherDefinitions)
			{
				skipOtherDefinitions = true;
			}

			if (result.RepeatForEachDefinitionEntry)
			{
				repeatForEachDefinitionEntry = true;
			}
		}

		result.Content = sb.Replace("Entrys", "Entries").Replace("Statuss", "Status").ToString();
		result.SkipOtherDefinitions = skipOtherDefinitions;
		result.RepeatForEachDefinitionEntry = repeatForEachDefinitionEntry;

		return result;
	}
}