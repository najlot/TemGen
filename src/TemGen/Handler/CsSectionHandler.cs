using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class CsSectionHandler : AbstractSectionHandler
{
	static CsSectionHandler()
	{
		_initialScript = CSharpScript.Create("", _options, typeof(Globals), _loader);
		_initialScript.Compile();
	}

	private static readonly Script<object> _initialScript = null;

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

	public override async Task Handle(Globals globals, TemplateSection section)
	{
		if (section.Handler != TemplateHandler.CSharp)
		{
			await Next.Handle(globals, section).ConfigureAwait(false);
			return;
		}

		if (!_cache.TryGetValue(section.Content, out var script))
		{
			script = _initialScript
				.ContinueWith(section.Content, _options)
				.CreateDelegate();

			_cache.TryAdd(section.Content, script);
		}

		await script(globals).ConfigureAwait(false);
	}
}