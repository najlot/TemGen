using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class PySectionHandler : AbstractSectionHandler
{
	private static readonly ScriptEngine _engine = IronPython.Hosting.Python.CreateEngine();
	private static readonly ConcurrentDictionary<string, ScriptSource> _cache = new();

	public override async Task Handle(Globals globals, TemplateSection section)
	{
		if (section.Handler != TemplateHandler.Python)
		{
			await Next.Handle(globals, section).ConfigureAwait(false);
			return;
		}
		/*
		var dc = new System.Collections.Generic.Dictionary<string, object>
		{
			["relative_path"] = globals.RelativePath,
			["definition"] = globals.Definition,
			["definition_entry"] = globals.DefinitionEntry,
			["entries"] = globals.Definition.Entries.ToArray(),
			["definitions"] = globals.Definitions.ToArray(),
			["skip_other_definitions"] = globals.SkipOtherDefinitions,
			["repeat_for_each_definition_entry"] = globals.RepeatForEachDefinitionEntry,
			["project"] = globals.Project,
			["get_result"] = () => globals.Result,
			["set_result"] = (Action<object>)(o => globals.Result = o.ToString()),
			["write"] = (Action<object>)globals.Write,
			["write_line"] = (Action<object>)globals.WriteLine,
		};
		*/
		var scope = _engine.CreateScope();
		
		scope.SetVariable("relative_path", globals.RelativePath);
		scope.SetVariable("definition", globals.Definition);
		scope.SetVariable("definition_entry", globals.DefinitionEntry);
		scope.SetVariable("entries", globals.Definition.Entries.ToArray());
		scope.SetVariable("definitions", globals.Definitions.ToArray());
		scope.SetVariable("skip_other_definitions", globals.SkipOtherDefinitions);
		scope.SetVariable("repeat_for_each_definition_entry", globals.RepeatForEachDefinitionEntry);
		scope.SetVariable("project", globals.Project);

		scope.SetVariable("get_result", () => globals.Result);
		scope.SetVariable("set_result", (Action<object>)(o => globals.Result = o.ToString()));

		scope.SetVariable("write", (Action<object>)globals.Write);
		scope.SetVariable("write_line", (Action<object>)globals.WriteLine);
		
		var source = _cache.GetOrAdd(section.Content, _engine.CreateScriptSourceFromString);
		source.Execute(scope);

		globals.RelativePath = scope.GetVariable<string>("relative_path");
		globals.SkipOtherDefinitions = scope.GetVariable<bool>("skip_other_definitions");
		globals.RepeatForEachDefinitionEntry = scope.GetVariable<bool>("repeat_for_each_definition_entry");
	}
}