using Acornima;
using Acornima.Ast;
using Jint;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class JintSectionHandler : AbstractSectionHandler
{
	private readonly Parser _parser = new();
	private static readonly ConcurrentDictionary<string, Prepared<Script>> _cache = new();

	public override async Task Handle(Globals globals, TemplateSection section)
	{
		if (section.Handler != TemplateHandler.JavaScript)
		{
			await Next.Handle(globals, section).ConfigureAwait(false);
			return;
		}

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

		var programm = _cache.GetOrAdd(section.Content, c => Engine.PrepareScript(c));
		engine.Execute(programm);

		globals.RelativePath = engine.GetValue("relativePath").ToString();
		globals.SkipOtherDefinitions = engine.GetValue("skipOtherDefinitions").AsBoolean();
		globals.RepeatForEachDefinitionEntry = engine.GetValue("repeatForEachDefinitionEntry").AsBoolean();
	}
}