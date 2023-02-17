using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class CsSectionHandler : AbstractSectionHandler
{
	private readonly Project _project;
	private readonly List<Definition> _definitions;

	public CsSectionHandler(Project project, List<Definition> definitions)
	{
		_project = project;
		_definitions = definitions;

		_initialScript = CSharpScript.Create("", _options, typeof(Globals), _loader);
		_initialScript.Compile();
	}

	private readonly Script<object> _initialScript = null;

	private static readonly ConcurrentDictionary<string, ScriptRunner<object>> _cache = new();
	private static readonly InteractiveAssemblyLoader _loader = GetLoader();
	private static readonly ScriptOptions _options = GetOptions();

	private static System.Reflection.Assembly[] GetReferences() => new[]
	{
		typeof(object).Assembly,
		typeof(System.IO.FileInfo).Assembly,
		typeof(System.Linq.IQueryable).Assembly,
		typeof(System.Dynamic.DynamicObject).Assembly,
		typeof(System.Text.RegularExpressions.Regex).Assembly
	};

	private static InteractiveAssemblyLoader GetLoader()
	{
		var loader = new InteractiveAssemblyLoader();

		foreach (var reference in GetReferences())
		{
			loader.RegisterDependency(reference);
		}

		return loader;
	}

	private static ScriptOptions GetOptions()
	{
		return ScriptOptions.Default
				.WithReferences(GetReferences())
				.AddImports(
					"System",
					"System.IO",
					"System.Linq",
					"System.Text",
					"System.Dynamic",
					"System.Collections.Generic",
					"System.Text.RegularExpressions"
					);
	}

	public override async Task<HandlingResult> Handle(
		TemplateSection section,
		Definition definition,
		string relativePath,
		DefinitionEntry definitionEntry)
	{
		if (section.Handler != TemplateHandler.CSharp)
		{
			return await Next.Handle(section, definition, relativePath, definitionEntry).ConfigureAwait(false);
		}

		var globals = new Globals()
		{
			RelativePath = relativePath,
			Definition = definition,
			Definitions = _definitions,
			DefinitionEntry = definitionEntry,
			Entries = definition.Entries,
			SkipOtherDefinitions = false,
			Project = _project,
			RepeatForEachDefinitionEntry = false
		};

		if (!_cache.TryGetValue(section.Content, out var script))
		{
			script = _initialScript
				.ContinueWith(section.Content, _options)
				.CreateDelegate();

			_cache.TryAdd(section.Content, script);
		}

		await script(globals).ConfigureAwait(false);

		return new HandlingResult()
		{
			RelativePath = globals.RelativePath,
			Content = globals.Result,
			SkipOtherDefinitions = globals.SkipOtherDefinitions,
			RepeatForEachDefinitionEntry = globals.RepeatForEachDefinitionEntry
		};
	}
}