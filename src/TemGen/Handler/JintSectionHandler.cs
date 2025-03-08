using Acornima.Ast;
using Jint;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class JintSectionHandler(string[] initialScripts) : AbstractSectionHandler(TemplateHandler.JavaScript)
{
	private static readonly ConcurrentDictionary<string, Prepared<Script>> _cache = new();

	protected override Task Handle(Globals globals, string content)
	{
		var engine = new Engine(cfg => cfg.LimitRecursion(1_000_000))
			.SetValue("relativePath", globals.RelativePath)
			.SetValue("definition", globals.Definition)
			.SetValue("definitionEntry", globals.DefinitionEntry)
			.SetValue("entries", globals.Definition.Entries.ToArray())
			.SetValue("definitions", globals.Definitions.ToArray())
			.SetValue("skipOtherDefinitions", globals.SkipOtherDefinitions)
			.SetValue("repeatForEachDefinitionEntry", globals.RepeatForEachDefinitionEntry)
			.SetValue("project", globals.Project)
			.SetValue("getResult", () => globals.Result)
			.SetValue("setResult", (Action<object>)(o => globals.Result = o.ToString()))
			.SetValue("write", (Action<object>)(o => globals.Write(o)))
			.SetValue("writeLine", (Action<object>)(o => globals.WriteLine(o)));

		foreach (var script in initialScripts)
		{
			var subScript = _cache.GetOrAdd(script, c => Engine.PrepareScript(c));
			engine.Execute(subScript);
		}

		var programm = _cache.GetOrAdd(content, c => Engine.PrepareScript(c));
		engine.Execute(programm);

		globals.RelativePath = engine.GetValue("relativePath").ToString();
		globals.SkipOtherDefinitions = engine.GetValue("skipOtherDefinitions").AsBoolean();
		globals.RepeatForEachDefinitionEntry = engine.GetValue("repeatForEachDefinitionEntry").AsBoolean();

		return Task.CompletedTask;
	}
}