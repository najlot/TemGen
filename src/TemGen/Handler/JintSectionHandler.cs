using Acornima.Ast;
using Jint;
using Jint.Native;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using TemGen.Models;
using TemGen.Services;

namespace TemGen.Handler;

public sealed class JintSectionHandler(string[] initialScripts) : AbstractSectionHandler(TemplateHandler.Script, TemplateLanguage.JavaScript)
{
	private static readonly ConcurrentDictionary<string, Prepared<Script>> _cache = new();

	public Task<object> EvaluateExpression(Globals globals, string expression)
	{
		var engine = CreateEngine(globals);
		var result = engine.Evaluate(expression);
		return Task.FromResult(result.ToObject());
	}

	protected override Task Handle(Globals globals, string content)
	{
		var engine = CreateEngine(globals);
		var programm = _cache.GetOrAdd(content, c => Engine.PrepareScript(c));
		engine.Execute(programm);

		globals.RelativePath = engine.GetValue("relativePath").ToString();
		globals.SkipOtherDefinitions = engine.GetValue("skipOtherDefinitions").AsBoolean();
		globals.RepeatForEachDefinitionEntry = engine.GetValue("repeatForEachDefinitionEntry").AsBoolean();
		globals.AllowOverwrite = engine.GetValue("allowOverwrite").AsBoolean();

		return Task.CompletedTask;
	}

	private Engine CreateEngine(Globals globals)
	{
		var engine = new Engine(cfg => cfg.LimitRecursion(1_000_000))
			.SetValue("relativePath", globals.RelativePath)
			.SetValue("previousContent", globals.PreviousContent)
			.SetValue("definition", globals.Definition)
			.SetValue("definitionEntry", globals.DefinitionEntry)
			.SetValue("entries", globals.Definition.Entries.ToArray())
			.SetValue("definitions", globals.Definitions.ToArray())
			.SetValue("skipOtherDefinitions", globals.SkipOtherDefinitions)
			.SetValue("repeatForEachDefinitionEntry", globals.RepeatForEachDefinitionEntry)
			.SetValue("allowOverwrite", globals.AllowOverwrite)
			.SetValue("project", globals.Project)
			.SetValue("getResult", () => globals.Result)
			.SetValue("setResult", (Action<object>)(o => globals.Result = o.ToString()))
			.SetValue("write", (Action<object>)(o => globals.Write(o)))
			.SetValue("writeLine", (Action<object>)(o => globals.WriteLine(o)))
			.SetValue("skipRemaining", (Func<object>)globals.SkipRemaining)
			.SetValue("setVariable", (Action<string, object>)((name, value) => globals.SetVariable(name, value)))
			.SetValue("getVariable", (Func<string, object>)(name => globals.GetVariable(name)));

		foreach (var variableName in globals.GetVisibleVariableNames())
		{
			var value = globals.GetVariable(variableName);
			engine.SetValue(variableName, value is JsValue jsValue ? jsValue : JsValue.FromObject(engine, value));
		}

		foreach (var script in initialScripts)
		{
			var subScript = _cache.GetOrAdd(script, c => Engine.PrepareScript(c));
			engine.Execute(subScript);
		}

		return engine;
	}
}