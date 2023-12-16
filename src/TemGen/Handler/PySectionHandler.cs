using Jint;
using Python.Runtime;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class PySectionHandler : AbstractSectionHandler
{
	private static readonly SemaphoreSlim _semaphore = new(1, 1);

	private static async Task PythonDownload()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			// On Windows, to simplify setup: download the python nuget-package and extract in to the local directory
			var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			Runtime.PythonDLL = "python312.dll";
			string pythonPath = Path.Combine(assemblyPath, Runtime.PythonDLL);
			if (!File.Exists(pythonPath))
			{
				using var client = new HttpClient()
				{
					BaseAddress = new Uri("https://www.nuget.org")
				};
				var response = await client.GetStreamAsync("/api/v2/package/python/3.12.1").ConfigureAwait(false);
				using var memStr = new MemoryStream();
				await response.CopyToAsync(memStr).ConfigureAwait(false);
				using var archive = new System.IO.Compression.ZipArchive(memStr);
				foreach (var entry in archive.Entries)
				{
					if (entry.FullName.StartsWith("tools"))
					{
						var path = Path.Combine(assemblyPath, entry.FullName[6..]);
						Directory.CreateDirectory(Path.GetDirectoryName(path));

						using var str = entry.Open();
						using var f = File.OpenWrite(path);
						await str.CopyToAsync(f).ConfigureAwait(false);
					}
				}
			}
		}
	}

	public override async Task Handle(Globals globals, TemplateSection section)
	{
		if (section.Handler != TemplateHandler.Python)
		{
			await Next.Handle(globals, section).ConfigureAwait(false);
			return;
		}

		if (!PythonEngine.IsInitialized)
		{
			await _semaphore.WaitAsync();

			try
			{
				if (!PythonEngine.IsInitialized)
				{
					await PythonDownload();

					PythonEngine.Initialize();
					PythonEngine.BeginAllowThreads();
				}
			}
			finally
			{
				_semaphore.Release();
			}
		}

		using (Py.GIL())
		{
			using var scope = Py.CreateScope();
			scope.Set("relative_path", globals.RelativePath);
			scope.Set("definition", globals.Definition);
			scope.Set("definition_entry", globals.DefinitionEntry);
			scope.Set("entries", globals.Definition.Entries.ToArray());
			scope.Set("definitions", globals.Definitions.ToArray());
			scope.Set("skip_other_definitions", globals.SkipOtherDefinitions);
			scope.Set("repeat_for_each_definition_entry", globals.RepeatForEachDefinitionEntry);
			scope.Set("project", globals.Project);

			scope.Set("get_result", () => globals.Result);
			scope.Set("set_result", (Action<object>)(o => globals.Result = o.ToString()));

			scope.Set("write", (Action<object>)(o => globals.Write(o)));
			scope.Set("write_line", (Action<object>)(o => globals.WriteLine(o)));

			scope.Exec(section.Content);

			globals.RelativePath = scope.Get("relative_path").ToString();
			globals.SkipOtherDefinitions = scope.Get("skip_other_definitions").As<bool>();
			globals.RepeatForEachDefinitionEntry = scope.Get("repeat_for_each_definition_entry").As<bool>();
		}
	}
}