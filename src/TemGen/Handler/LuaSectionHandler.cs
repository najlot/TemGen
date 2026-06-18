using MoonSharp.Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemGen.Models;
using TemGen.Services;

namespace TemGen.Handler;

public sealed class LuaSectionHandler(string[] initialScripts) : AbstractSectionHandler(TemplateHandler.Script, TemplateLanguage.Lua)
{
	public Task<object> EvaluateExpression(Globals globals, string expression)
	{
		var script = CreateScript(globals);
		var result = script.DoString($"return {expression}");
		return Task.FromResult(ConvertDynValueToHostObject(result));
	}

	protected override Task Handle(Globals globals, string content)
	{
		var script = CreateScript(globals);

		script.DoString(content);

		globals.RelativePath = script.Globals["relative_path"].ToString();
		globals.SkipOtherDefinitions = (bool)script.Globals["skip_other_definitions"];
		globals.RepeatForEachDefinitionEntry = (bool)script.Globals["repeat_for_each_definition_entry"];
		globals.AllowOverwrite = (bool)script.Globals["allow_overwrite"];

		return Task.CompletedTask;
	}

	private Script CreateScript(Globals globals)
	{
		var script = new Script();

		script.Globals["relative_path"] = globals.RelativePath;
		script.Globals["previous_content"] = globals.PreviousContent;

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
		script.Globals["allow_overwrite"] = globals.AllowOverwrite;

		var projectTable = new Table(script);
		projectTable["namespace"] = globals.Project.Namespace;
		projectTable["project_directory"] = globals.Project.ProjectDirectory;
		projectTable["definitions_path"] = globals.Project.DefinitionsPath;
		projectTable["templates_path"] = globals.Project.TemplatesPath;
		projectTable["output_path"] = globals.Project.OutputPath;
		projectTable["resources_path"] = globals.Project.ResourcesPath;
		projectTable["resources_script_path"] = globals.Project.ResourcesScriptPath;
		projectTable["scripts_path"] = globals.Project.ScriptsPath;
		projectTable["primary_color"] = globals.Project.PrimaryColor;
		projectTable["primary_dark_color"] = globals.Project.PrimaryDarkColor;
		projectTable["accent_color"] = globals.Project.AccentColor;
		projectTable["foreground_color"] = globals.Project.ForegroundColor;

		var settingsTable = new Table(script);

		foreach (var setting in globals.Project.Settings)
		{
			settingsTable[setting.Key] = setting.Value;
			projectTable[setting.Key] = setting.Value;
		}

		projectTable["settings"] = settingsTable;
		script.Globals["project"] = projectTable;

		script.Globals["get_result"] = () => globals.Result;
		script.Globals["set_result"] = (Action<object>)(o => globals.Result = o.ToString());

		script.Globals["write"] = (Action<object>)(o => globals.Write(o));
		script.Globals["write_line"] = (Action<object>)(o => globals.WriteLine(o));
		script.Globals["skip_remaining"] = (Func<object>)globals.SkipRemaining;

		script.Globals["set_variable"] = (Action<string, object>)((name, value) => globals.SetVariable(name, value));
		script.Globals["get_variable"] = (Func<string, object>)(name => globals.GetVariable(name));

		foreach (var variableName in globals.GetVisibleVariableNames())
		{
			script.Globals[variableName] = ToDynValue(script, globals.GetVariable(variableName));
		}

		foreach (var subScript in initialScripts)
		{
			script.DoString(subScript);
		}

		return script;
	}

	private static object ConvertDynValueToHostObject(DynValue value)
	{
		return value.Type switch
		{
			DataType.Void or DataType.Nil => null,
			DataType.Boolean => value.Boolean,
			DataType.Number => value.Number,
			DataType.String => value.String,
			DataType.Table => ConvertTableToHostObject(value.Table),
			_ => value.ToObject()
		};
	}

	private static object ConvertTableToHostObject(Table table)
	{
		if (table.Length > 0)
		{
			var values = new object[(int)table.Length];

			for (int index = 1; index <= table.Length; index++)
			{
				values[index - 1] = ConvertDynValueToHostObject(table.Get(index));
			}

			return values;
		}

		var valuesByKey = new Dictionary<string, object>(StringComparer.Ordinal);

		foreach (var pair in table.Pairs)
		{
			var key = ConvertDynValueToHostObject(pair.Key)?.ToString();

			if (!string.IsNullOrEmpty(key))
			{
				valuesByKey[key] = ConvertDynValueToHostObject(pair.Value);
			}
		}

		return valuesByKey;
	}

	private static DynValue ToDynValue(Script script, object value)
	{
		return value switch
		{
			null => DynValue.Nil,
			DynValue dynValue => ToDynValue(script, ConvertDynValueToHostObject(dynValue)),
			IDictionary dictionary => ToTable(script, dictionary),
			IEnumerable<KeyValuePair<string, object>> pairs => ToTable(script, pairs),
			IEnumerable enumerable when value is not string => ToArrayTable(script, enumerable),
			_ => DynValue.FromObject(script, value)
		};
	}

	private static DynValue ToTable(Script script, IDictionary values)
	{
		var table = new Table(script);

		foreach (DictionaryEntry entry in values)
		{
			if (entry.Key is not null)
			{
				table[entry.Key.ToString()] = ToDynValue(script, entry.Value);
			}
		}

		return DynValue.NewTable(table);
	}

	private static DynValue ToTable(Script script, IEnumerable<KeyValuePair<string, object>> pairs)
	{
		var table = new Table(script);

		foreach (var pair in pairs)
		{
			table[pair.Key] = ToDynValue(script, pair.Value);
		}

		return DynValue.NewTable(table);
	}

	private static DynValue ToArrayTable(Script script, IEnumerable values)
	{
		var table = new Table(script);
		var index = 1;

		foreach (var value in values)
		{
			table[index++] = ToDynValue(script, value);
		}

		return DynValue.NewTable(table);
	}
}