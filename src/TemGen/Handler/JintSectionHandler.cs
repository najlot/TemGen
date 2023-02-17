using Jint;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class JintSectionHandler : AbstractSectionHandler
{
	private readonly Project _project;
	private readonly List<Definition> _definitions;

	public JintSectionHandler(Project project, List<Definition> definitions)
	{
		_project = project;
		_definitions = definitions;
	}

	private readonly Jint.Parser.JavaScriptParser _parser = new();
	private static readonly ConcurrentDictionary<string, Jint.Parser.Ast.Program> _cache = new();

	public override async Task<HandlingResult> Handle(TemplateSection section, Definition definition, string relativePath, DefinitionEntry definitionEntry)
	{
		if (section.Handler != TemplateHandler.JavaScript)
		{
			return await Next.Handle(section, definition, relativePath, definitionEntry).ConfigureAwait(false);
		}

		var engine = new Engine(cfg => cfg.LimitRecursion(1_000_000))
			.SetValue("relativePath", relativePath)
			.SetValue("definition", definition)
			.SetValue("definitionEntry", definitionEntry)
			.SetValue("entries", definition.Entries.ToArray())
			.SetValue("definitions", _definitions.ToArray())
			.SetValue("skipOtherDefinitions", false)
			.SetValue("repeatForEachDefinitionEntry", false)
			.SetValue("project", _project)
			.SetValue("result", "");

		engine.SetValue("write", (Action<object>)(o =>
			{
				var val = engine.GetValue("result");
				engine.SetValue("result", $"{val}{o}");
			}))
			.SetValue("writeLine", (Action<object>)(o =>
			{
				var val = engine.GetValue("result");
				engine.SetValue("result", $"{val}{o}{Environment.NewLine}");
			}));

		if (!_cache.TryGetValue(section.Content, out var programm))
		{
			programm = _parser.Parse(section.Content);
			_cache.TryAdd(section.Content, programm);
		}

		engine.Execute(programm);

		return new HandlingResult()
		{
			RelativePath = engine.GetValue("relativePath").ToString(),
			Content = engine.GetValue("result").ToString(),
			SkipOtherDefinitions = (bool)engine.GetValue("skipOtherDefinitions").ToObject(),
			RepeatForEachDefinitionEntry = (bool)engine.GetValue("repeatForEachDefinitionEntry").ToObject(),
		};
	}
}