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

	public CsSectionHandler() : base(TemplateHandler.CSharp) { }

	private static readonly Script<object> _initialScript = null;

	private static readonly ConcurrentDictionary<string, ScriptRunner<object>> _cache = new();
	private static readonly InteractiveAssemblyLoader _loader = GetLoader();
	private static readonly ScriptOptions _options = GetOptions();

	private static System.Reflection.Assembly[] GetReferences() =>
	[
		typeof(object).Assembly,
		typeof(System.IO.FileInfo).Assembly,
		typeof(System.Linq.IQueryable).Assembly,
		typeof(System.Dynamic.DynamicObject).Assembly,
		typeof(System.Text.RegularExpressions.Regex).Assembly
	];

	private static InteractiveAssemblyLoader GetLoader()
	{
		var loader = new InteractiveAssemblyLoader();

		foreach (var reference in GetReferences())
		{
			loader.RegisterDependency(reference);
		}

		return loader;
	}

	private static ScriptOptions GetOptions() =>
		ScriptOptions.Default
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

	protected override async Task Handle(Globals globals, string content)
	{
		var script = _cache.GetOrAdd(content, c => _initialScript.ContinueWith(c, _options).CreateDelegate());
		await script(globals).ConfigureAwait(false);
	}
}