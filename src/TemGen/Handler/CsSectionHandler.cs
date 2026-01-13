using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class CsSectionHandler : AbstractSectionHandler
{
	#region Static Initialization

	private static readonly Script<object> _emptyScript = null;

	static CsSectionHandler()
	{
		_emptyScript = CSharpScript.Create(string.Empty, _options, typeof(Globals), _loader);
		_emptyScript.Compile();
	}

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

	#endregion Static Initialization

	private static readonly ConcurrentDictionary<(int ScriptsKey, string Content), ScriptRunner<object>> _cache = new();

	private readonly Script<object> _initialScript = null;
	private readonly int _initialScriptsKey;

	public CsSectionHandler(string[] initialScripts) : base(TemplateHandler.CSharp)
	{
		_initialScript = _emptyScript;

		foreach (var script in initialScripts)
		{
			_initialScript = _initialScript.ContinueWith(script, _options);
		}

		_initialScriptsKey = string.Join('\n', initialScripts).GetHashCode();
	}

	protected override async Task Handle(Globals globals, string content)
	{
		var script = _cache.GetOrAdd((_initialScriptsKey, content), c => _initialScript.ContinueWith(c.Content, _options).CreateDelegate());
		await script(globals).ConfigureAwait(false);
	}
}