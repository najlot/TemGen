using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class PySectionHandler : AbstractSectionHandler
{
	private readonly Project _project;
	private readonly List<Definition> _definitions;
	private static bool _pyReady = false;

	public PySectionHandler(Project project, List<Definition> definitions)
	{
		_project = project;
		_definitions = definitions;
	}

	private static void PythonDownload()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			// On Windows, to simplify setup: download the python nuget-package and extract in to the local directory
			var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			if (!File.Exists(Path.Combine(assemblyPath, "python37.dll")))
			{
				using var client = new HttpClient()
				{
					BaseAddress = new Uri("https://www.nuget.org")
				};
				var response = client.GetStreamAsync("/api/v2/package/python/3.7.6").Result;
				using var memStr = new MemoryStream();
				response.CopyTo(memStr);
				using var archive = new System.IO.Compression.ZipArchive(memStr);
				foreach (var entry in archive.Entries)
				{
					if (entry.FullName.StartsWith("tools"))
					{
						var path = Path.Combine(assemblyPath, entry.FullName[6..]);
						Directory.CreateDirectory(Path.GetDirectoryName(path));

						using var str = entry.Open();
						using var f = File.OpenWrite(path);
						str.CopyTo(f);
					}
				}
			}
		}


	}

	public override async Task<HandlingResult> Handle(
		TemplateSection section,
		Definition definition,
		string relativePath,
		DefinitionEntry definitionEntry)
	{
		if (section.Handler != TemplateHandler.Python)
		{
			return await Next.Handle(section, definition, relativePath, definitionEntry).ConfigureAwait(false);
		}

		if (!_pyReady)
		{
			lock (typeof(PySectionHandler))
			{
				PythonDownload();

				PythonEngine.Initialize();
				PythonEngine.BeginAllowThreads();

				_pyReady = true;
			}
		}

		using (Py.GIL())
		{
			using var scope = Py.CreateScope();
			scope.Set("relativePath", relativePath);
			scope.Set("definition", definition);
			scope.Set("definitionEntry", definitionEntry);
			scope.Set("entries", definition.Entries.ToArray());
			scope.Set("definitions", _definitions.ToArray());
			scope.Set("skipOtherDefinitions", false);
			scope.Set("repeatForEachDefinitionEntry", false);
			scope.Set("project", _project);
			scope.Set("result", "");

			scope.Set("write", (Action<object>)(o =>
			{
				var val = scope.Get("result");
				scope.Set("result", $"{val}{o}");
			}));

			scope.Set("writeLine", (Action<object>)(o =>
			{
				var val = scope.Get("result");
				scope.Set("result", $"{val}{o}{Environment.NewLine}");
			}));

			scope.Exec(section.Content);

			return new HandlingResult()
			{
				RelativePath = scope.Get("relativePath").ToString(),
				Content = scope.Get("result").ToString(),
				SkipOtherDefinitions = scope.Get("skipOtherDefinitions").As<bool>(),
				RepeatForEachDefinitionEntry = scope.Get("repeatForEachDefinitionEntry").As<bool>(),
			};
		}
	}
}