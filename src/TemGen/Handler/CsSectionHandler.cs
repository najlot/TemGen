using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class CsSectionHandler : AbstractSectionHandler
{
	#region Static Initialization

	private static readonly InteractiveAssemblyLoader _loader;
	private static readonly ScriptOptions _options;
	private static readonly Script<object> _emptyScript;

	static CsSectionHandler()
	{
		_loader = GetLoader();
		_options = GetOptions();
		_emptyScript = CSharpScript.Create(string.Empty, _options, typeof(Globals), _loader);
		_emptyScript.Compile();
	}

	private static System.Reflection.Assembly[] GetReferences() =>
	[
		typeof(object).Assembly,
		typeof(System.IO.FileInfo).Assembly,
		typeof(System.Linq.IQueryable).Assembly,
		typeof(System.Dynamic.DynamicObject).Assembly,
		typeof(System.Text.RegularExpressions.Regex).Assembly,
		typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly
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

	private static readonly ConcurrentDictionary<(string ScriptsKey, string Content), ScriptRunner<object>> _cache = new();
	private static readonly ConcurrentDictionary<(string ScriptsKey, string Content), ScriptRunner<object>> _expressionCache = new();

	private readonly Script<object> _initialScript;
	private readonly string _initialScriptsKey;

	public CsSectionHandler(string[] initialScripts) : base(TemplateHandler.CSharp)
	{
		_initialScript = _emptyScript;

		foreach (var script in initialScripts)
		{
			_initialScript = _initialScript.ContinueWith(script, _options);
		}

		_initialScriptsKey = GetStableCacheKey(initialScripts);
	}

	public async Task<object> EvaluateExpression(Globals globals, string expression)
	{
		var content = WrapWithVisibleVariables(globals, $"return (object)({expression});");

		var scriptRunner = _expressionCache.GetOrAdd(
			(_initialScriptsKey, content),
			static (key, state) => state._initialScript.ContinueWith(key.Content, _options).CreateDelegate(),
			this);

		return await scriptRunner(globals).ConfigureAwait(false);
	}

	protected override async Task Handle(Globals globals, string content)
	{
		content = EnsureStatementTermination(content);
		content = WrapWithVisibleVariables(globals, content + Environment.NewLine + "return null;");

		var scriptRunner = _cache.GetOrAdd(
			(_initialScriptsKey, content),
			static (key, state) => state._initialScript.ContinueWith(key.Content, _options).CreateDelegate(),
			this);

		await scriptRunner(globals).ConfigureAwait(false);
	}

	private static string WrapWithVisibleVariables(Globals globals, string content)
	{
		var variableNames = globals.GetVisibleVariableNames();

		if (variableNames.Count == 0)
		{
			return content;
		}

		var declarations = string.Join(Environment.NewLine, variableNames.Select(name => $"dynamic {name} = GetVariable(\"{name}\");"));
		return declarations + Environment.NewLine + content;
	}

	private static string EnsureStatementTermination(string content)
	{
		var trimmedContent = content.TrimEnd();

		if (trimmedContent.EndsWith(';') || trimmedContent.EndsWith('}'))
		{
			return content;
		}

		return content + ";";
	}

	private static string GetStableCacheKey(string[] scripts)
	{
		if (scripts == null || scripts.Length == 0)
		{
			return string.Empty;
		}

		var content = string.Join('\n', scripts);
		var hash = SHA256.HashData(Encoding.UTF8.GetBytes(content));
		return Convert.ToHexString(hash);
	}
}