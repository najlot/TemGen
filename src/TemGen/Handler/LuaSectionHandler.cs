using MoonSharp.Interpreter;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class LuaSectionHandler : AbstractSectionHandler
{
	public override async Task Handle(Globals globals, TemplateSection section)
	{
		if (section.Handler != TemplateHandler.Lua)
		{
			await Next.Handle(globals, section).ConfigureAwait(false);
			return;
		}

		var script = new Script();

		script.Globals["relative_path"] = globals.RelativePath;

		var definitionTable = new Table(script);
		definitionTable["name"] = globals.Definition.Name;
		definitionTable["name_low"] = globals.Definition.NameLow;
		definitionTable["is_owned_type"] = globals.Definition.IsOwnedType;
		definitionTable["is_enumeration"] = globals.Definition.IsEnumeration;
		definitionTable["is_array"] = globals.Definition.IsArray;
		script.Globals["definition"] = definitionTable;
		script.Globals["definition_entry"] = globals.DefinitionEntry;

		script.Globals["entries"] = globals.Definition.Entries.Select(entry =>
		{
			var entryTable = new Table(script);

			entryTable["entry_type"] = entry.EntryType;
			entryTable["field"] = entry.Field;
			entryTable["field_low"] = entry.FieldLow;
			entryTable["is_owned_type"] = entry.IsOwnedType;
			entryTable["is_key"] = entry.IsKey;
			entryTable["is_array"] = entry.IsArray;
			entryTable["is_reference"] = entry.IsReference;
			entryTable["is_enumeration"] = entry.IsEnumeration;
			entryTable["is_nullable"] = entry.IsNullable;
			entryTable["reference_type"] = entry.ReferenceType;
			entryTable["reference_type_low"] = entry.ReferenceTypeLow;

			return entryTable;
		}).ToArray();

		script.Globals["definitions"] = globals.Definitions.Select(definition =>
		{
			var definitionTable = new Table(script);

			definitionTable["name"] = definition.Name;
			definitionTable["name_low"] = definition.NameLow;
			definitionTable["is_owned_type"] = definition.IsOwnedType;
			definitionTable["is_enumeration"] = definition.IsEnumeration;
			definitionTable["is_array"] = definition.IsArray;

			return definitionTable;
		}).ToArray();

		script.Globals["skip_other_definitions"] = globals.SkipOtherDefinitions;
		script.Globals["repeat_for_each_definition_entry"] = globals.RepeatForEachDefinitionEntry;

		var projectTable = new Table(script);
		projectTable["namespace"] = globals.Project.Namespace;
		projectTable["project_directory"] = globals.Project.ProjectDirectory;
		projectTable["definitions_path"] = globals.Project.DefinitionsPath;
		projectTable["templates_path"] = globals.Project.TemplatesPath;
		projectTable["output_path"] = globals.Project.OutputPath;
		projectTable["primary_color"] = globals.Project.PrimaryColor;
		projectTable["primary_dark_color"] = globals.Project.PrimaryDarkColor;
		projectTable["accent_color"] = globals.Project.AccentColor;
		projectTable["foreground_color"] = globals.Project.ForegroundColor;
		script.Globals["project"] = projectTable;

		script.Globals["get_result"] = () => globals.Result;
		script.Globals["set_result"] = (Action<object>)(o => globals.Result = o.ToString());

		script.Globals["write"] = (Action<object>)(o => globals.Write(o));
		script.Globals["write_line"] = (Action<object>)(o => globals.WriteLine(o));

		script.DoString(section.Content);

		globals.RelativePath = script.Globals["relative_path"].ToString();
		globals.SkipOtherDefinitions = (bool)script.Globals["skip_other_definitions"];
		globals.RepeatForEachDefinitionEntry = (bool)script.Globals["repeat_for_each_definition_entry"];
	}
}