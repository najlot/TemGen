using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class PySectionHandler : AbstractSectionHandler
{
	public PySectionHandler() : base(TemplateHandler.Python) { }

	private static readonly ScriptEngine _engine = IronPython.Hosting.Python.CreateEngine();
	private static readonly ConcurrentDictionary<string, ScriptSource> _cache = new();

	protected override Task Handle(Globals globals, string content)
	{
		var scriptGlobals = new Dictionary<string, object>
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

		var scope = _engine.CreateScope(scriptGlobals);
		var source = _cache.GetOrAdd(content, _engine.CreateScriptSourceFromString);
		source.Execute(scope);

		globals.RelativePath = (string)scriptGlobals["relative_path"];
		globals.SkipOtherDefinitions = (bool)scriptGlobals["skip_other_definitions"];
		globals.RepeatForEachDefinitionEntry = (bool)scriptGlobals["repeat_for_each_definition_entry"];

		return Task.CompletedTask;
	}
}