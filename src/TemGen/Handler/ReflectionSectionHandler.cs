using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class ReflectionSectionHandler : AbstractSectionHandler
{
	private readonly Project _project;
	private readonly List<Definition> _definitions;

	public ReflectionSectionHandler(Project project, List<Definition> definitions)
	{
		_project = project;
		_definitions = definitions;
	}

	private string GetValue(IEnumerable<string> parts, object obj)
	{
		if (obj is null)
		{
			return "";
		}

		if (!parts.Any())
		{
			return obj?.ToString() ?? "";
		}

		var type = obj.GetType();
		var propertyName = parts.First();

		if (string.IsNullOrEmpty(propertyName))
		{
			throw new Exception("Property name is null or empty. Expression ends with '.'?");
		}

		var property = type.GetProperty(propertyName);

		if (property is null)
		{
			throw new Exception("Property '" + propertyName + "' not found.");
		}

		obj = property.GetValue(obj, null);

		return GetValue(parts.Skip(1), obj);
	}

	public override async Task<HandlingResult> Handle(TemplateSection section, Definition definition, string relativePath, DefinitionEntry definitionEntry)
	{
		if (section.Handler != TemplateHandler.Reflection)
		{
			return await Next.Handle(section, definition, relativePath, definitionEntry).ConfigureAwait(false);
		}

		var globals = new Globals()
		{
			RelativePath = relativePath,
			Definition = definition,
			Definitions = _definitions,
			DefinitionEntry = definitionEntry,
			Entries = definition.Entries,
			SkipOtherDefinitions = false,
			Project = _project,
		};

		var parts = section.Content.Trim().Split('.');
		var value = GetValue(parts, globals);

		return new HandlingResult()
		{
			RelativePath = relativePath,
			Content = value,
		};
	}
}