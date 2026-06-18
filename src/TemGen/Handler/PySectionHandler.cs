using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemGen.Models;
using TemGen.Services;

namespace TemGen.Handler;

public sealed class PySectionHandler(string[] initialScripts) : AbstractSectionHandler(TemplateHandler.Script, TemplateLanguage.Python)
{
	private static readonly ScriptEngine _engine = IronPython.Hosting.Python.CreateEngine();
	private static readonly ConcurrentDictionary<string, ScriptSource> _cache = new();
	private static readonly ConcurrentDictionary<string, ScriptSource> _expressionCache = new();

	public Task<object> EvaluateExpression(Globals globals, string expression)
	{
		var (scope, _) = CreateScope(globals);
		var source = _expressionCache.GetOrAdd(expression, static content => _engine.CreateScriptSourceFromString(content, SourceCodeKind.Expression));
		return Task.FromResult((object)source.Execute(scope));
	}

	protected override Task Handle(Globals globals, string content)
	{
		var (scope, scriptGlobals) = CreateScope(globals);

		foreach (var script in initialScripts)
		{
			var subSource = _cache.GetOrAdd(script, _engine.CreateScriptSourceFromString);
			subSource.Execute(scope);
		}

		var source = _cache.GetOrAdd(content, _engine.CreateScriptSourceFromString);
		source.Execute(scope);

		globals.RelativePath = (string)scriptGlobals["relative_path"];
		globals.SkipOtherDefinitions = (bool)scriptGlobals["skip_other_definitions"];
		globals.RepeatForEachDefinitionEntry = (bool)scriptGlobals["repeat_for_each_definition_entry"];
		globals.AllowOverwrite = (bool)scriptGlobals["allow_overwrite"];

		return Task.CompletedTask;
	}

	private static (ScriptScope Scope, Dictionary<string, object> ScriptGlobals) CreateScope(Globals globals)
	{
		var scriptGlobals = new Dictionary<string, object>
		{
			["relative_path"] = globals.RelativePath,
			["previous_content"] = globals.PreviousContent,
			["definition"] = globals.Definition,
			["definition_entry"] = globals.DefinitionEntry,
			["entries"] = globals.Definition.Entries.ToArray(),
			["definitions"] = globals.Definitions.ToArray(),
			["skip_other_definitions"] = globals.SkipOtherDefinitions,
			["repeat_for_each_definition_entry"] = globals.RepeatForEachDefinitionEntry,
			["allow_overwrite"] = globals.AllowOverwrite,
			["project"] = globals.Project,
			["get_result"] = () => globals.Result,
			["set_result"] = (Action<object>)(o => globals.Result = o.ToString()),
			["write"] = (Action<object>)globals.Write,
			["write_line"] = (Action<object>)globals.WriteLine,
			["skip_remaining"] = (Func<object>)globals.SkipRemaining,
			["set_variable"] = (Action<string, object>)((name, value) => globals.SetVariable(name, value)),
			["get_variable"] = (Func<string, object>)(name => globals.GetVariable(name))
		};

		foreach (var variableName in globals.GetVisibleVariableNames())
		{
			scriptGlobals[variableName] = globals.GetVariable(variableName);
		}

		return (_engine.CreateScope(scriptGlobals), scriptGlobals);
	}
}