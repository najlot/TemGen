using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class LuaSectionHandler : AbstractSectionHandler
{
	private readonly Project _project;
	private readonly List<Definition> _definitions;

	public LuaSectionHandler(Project project, List<Definition> definitions)
	{
		_project = project;
		_definitions = definitions;
	}

	public override async Task<HandlingResult> Handle(TemplateSection section, Definition definition, string relativePath, DefinitionEntry definitionEntry)
	{
		if (section.Handler != TemplateHandler.Lua)
		{
			return await Next.Handle(section, definition, relativePath, definitionEntry).ConfigureAwait(false);
		}

		var script = new Script();

		script.Globals["relativePath"] = relativePath;

		var definitionTable = new Table(script);
		definitionTable["name"] = definition.Name;
		definitionTable["nameLow"] = definition.NameLow;
		definitionTable["isOwnedType"] = definition.IsOwnedType;
		definitionTable["isEnumeration"] = definition.IsEnumeration;
		definitionTable["isArray"] = definition.IsArray;
		script.Globals["definition"] = definitionTable;
		script.Globals["definitionEntry"] = definitionEntry;

		script.Globals["entries"] = definition.Entries.Select(entry =>
		{
			var entryTable = new Table(script);

			entryTable["entryType"] = entry.EntryType;
			entryTable["field"] = entry.Field;
			entryTable["fieldLow"] = entry.FieldLow;
			entryTable["isOwnedType"] = entry.IsOwnedType;
			entryTable["isKey"] = entry.IsKey;
			entryTable["isArray"] = entry.IsArray;
			entryTable["isReference"] = entry.IsReference;
			entryTable["isEnumeration"] = entry.IsEnumeration;
			entryTable["isNullable"] = entry.IsNullable;
			entryTable["referenceType"] = entry.ReferenceType;
			entryTable["referenceTypeLow"] = entry.ReferenceTypeLow;

			return entryTable;
		}).ToArray();

		script.Globals["definitions"] = _definitions.Select(definition =>
		{
			var definitionTable = new Table(script);

			definitionTable["name"] = definition.Name;
			definitionTable["nameLow"] = definition.NameLow;
			definitionTable["isOwnedType"] = definition.IsOwnedType;
			definitionTable["isEnumeration"] = definition.IsEnumeration;
			definitionTable["isArray"] = definition.IsArray;

			return definitionTable;
		}).ToArray();

		script.Globals["skipOtherDefinitions"] = false;
		script.Globals["repeatForEachDefinitionEntry"] = false;

		var projectTable = new Table(script);
		projectTable["namespace"] = _project.Namespace;
		projectTable["ProjectDirectory"] = _project.ProjectDirectory;
		projectTable["DefinitionsPath"] = _project.DefinitionsPath;
		projectTable["TemplatesPath"] = _project.TemplatesPath;
		projectTable["OutputPath"] = _project.OutputPath;
		projectTable["PrimaryColor"] = _project.PrimaryColor;
		projectTable["PrimaryDarkColor"] = _project.PrimaryDarkColor;
		projectTable["AccentColor"] = _project.AccentColor;
		projectTable["ForegroundColor"] = _project.ForegroundColor;
		script.Globals["project"] = projectTable;

		script.Globals["result"] = "";

		script.Globals["write"] = (Action<object>)(o =>
		{
			var val = script.Globals["result"];
			script.Globals["result"] = $"{val}{o}";
		});

		script.Globals["writeLine"] = (Action<object>)(o =>
		{
			var val = script.Globals["result"];
			script.Globals["result"] = $"{val}{o}{Environment.NewLine}";
		});

		script.DoString(section.Content);

		return new HandlingResult()
		{
			RelativePath = script.Globals["relativePath"].ToString(),
			Content = script.Globals["result"].ToString(),
			SkipOtherDefinitions = (bool)script.Globals["skipOtherDefinitions"],
			RepeatForEachDefinitionEntry = (bool)script.Globals["repeatForEachDefinitionEntry"],
		};
	}
}