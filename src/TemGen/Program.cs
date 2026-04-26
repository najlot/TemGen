using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TemGen.Handler;

namespace TemGen;

internal class Program
{
	private static bool IsTemplateReadFailure(Exception ex)
		=> ex is TemplateReadException
			|| ex is AggregateException aggregate && aggregate.InnerExceptions.All(e => IsTemplateReadFailure(e));

	private static async Task Generate(string path)
	{
		var sw = Stopwatch.StartNew();

		Console.WriteLine("Starting execution...");

		var project = ProjectReader.ReadProject(path);
		var definitions = DefinitionsReader.ReadDefinitions(project.DefinitionsPath);
		
		var csScripts = Array.Empty<string>();
		var jsScripts = Array.Empty<string>();
		var pyScripts = Array.Empty<string>();
		var luaScripts = Array.Empty<string>();

		if (!string.IsNullOrWhiteSpace(project.ScriptsPath))
		{
			var scripts = TemplatesReader.ReadScripts(project.ScriptsPath);
			csScripts = scripts.Where(script => script.Handler == TemplateHandler.CSharp).Select(s => s.Content).ToArray();
			jsScripts = scripts.Where(script => script.Handler == TemplateHandler.JavaScript).Select(s => s.Content).ToArray();
			pyScripts = scripts.Where(script => script.Handler == TemplateHandler.Python).Select(s => s.Content).ToArray();
			luaScripts = scripts.Where(script => script.Handler == TemplateHandler.Lua).Select(s => s.Content).ToArray();
		}

		var processor = new TemplateProcessor([
			new TextSectionHandler(),
			new CsSectionHandler(csScripts),
			new ReflectionSectionHandler(),
			new PySectionHandler(pyScripts),
			new JintSectionHandler(jsScripts),
			new LuaSectionHandler(luaScripts)
		], project, definitions);
		var generatedFiles = new ConcurrentBag<string>();
		var hasProcessingErrors = false;
		GeneratedOutputManifest previousManifest = null;

		try
		{
			previousManifest = await GeneratedOutputManifest.LoadAsync(project.OutputPath).ConfigureAwait(false);
		}
		catch (JsonException ex)
		{
			Console.Error.WriteLine($"Could not read generation manifest {GeneratedOutputManifest.GetManifestPath(project.OutputPath)}. Stale-file cleanup will be skipped for this run. {ex}");
		}

		if (!string.IsNullOrWhiteSpace(project.ResourcesScriptPath))
		{
			var (handler, content) = TemplatesReader.ReadScript(path, project.ResourcesScriptPath);

			try
			{
				await processor.Handle(new Template
				{
					RelativePath = project.ResourcesScriptPath,
					Sections = [new TemplateSection { Handler = handler, Content = content }],
					Encoding = System.Text.Encoding.UTF8,
				}, new Definition() { Entries = [] }, null).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				hasProcessingErrors = true;
				Console.Error.WriteLine($"Error processing script {project.ResourcesScriptPath}: {ex}");
			}
		}

		foreach (var templatesPath in project.TemplatePaths)
		{
			List<Template> templates;

			try
			{
				templates = TemplatesReader.ReadTemplates(templatesPath);
			}
			catch (AggregateException ex) when (ex.InnerExceptions.All(e => e is TemplateReadException))
			{
				foreach (var error in ex.InnerExceptions.Cast<TemplateReadException>())
				{
					Console.Error.WriteLine($"Error reading template {error.TemplatePath}: {error.InnerException?.Message ?? error.Message}");
				}

				throw;
			}

			await Parallel.ForEachAsync(templates, async (template, tkn) =>
			{
				try
				{
					var results = await processor.Handle(template, definitions).ConfigureAwait(false);

					foreach (var (key, (encoding, content)) in results)
					{
						var destPath = Path.Combine(project.OutputPath, key);
						var normalizedRelativePath = GeneratedOutputManifest.NormalizeRelativePath(key);
						var dirPath = Path.GetDirectoryName(destPath);
						Directory.CreateDirectory(dirPath);

						if (File.Exists(destPath))
						{
							var currentContent = await File.ReadAllTextAsync(destPath, tkn).ConfigureAwait(false);
							if (currentContent != content)
							{
								await File.WriteAllTextAsync(destPath, content, encoding, tkn).ConfigureAwait(false);
							}
						}
						else
						{
							await File.WriteAllTextAsync(destPath, content, encoding, tkn).ConfigureAwait(false);
						}

						generatedFiles.Add(normalizedRelativePath);
					}
				}
				catch (Exception ex)
				{
					hasProcessingErrors = true;
					Console.Error.WriteLine($"Error processing {template.RelativePath}: {ex}");
				}
			}).ConfigureAwait(false);
		}

		if (hasProcessingErrors)
		{
			Console.WriteLine("Skipping stale-file cleanup because generation completed with errors.");
		}
		else
		{
			var currentManifest = new GeneratedOutputManifest
			{
				Files = generatedFiles.OrderBy(f => f, StringComparer.Ordinal).ToHashSet(StringComparer.Ordinal)
			};

			var deletedCount = GeneratedOutputCleaner.Cleanup(project.OutputPath, previousManifest, currentManifest);

			if (deletedCount > 0)
			{
				Console.WriteLine($"Stale-file cleanup removed {deletedCount} file(s).");
			}

			await currentManifest.SaveAsync(project.OutputPath).ConfigureAwait(false);
		}
		
		Console.WriteLine($"Done after {sw.Elapsed}.");
	}

	private static async Task<int> Main(string[] args)
	{
		var rootCommand = new RootCommand("TemGen - Template based code generator");

		var pathOption = new Option<string>("--path", "-p")
		{
			Description = "Path to a project definition file or a folder containing ProjectDefinition.json or ProjectDefinition.",
			DefaultValueFactory = (r) => "."
		};
		rootCommand.Add(pathOption);

		var loopOption = new Option<bool>("--loop", "-l")
		{
			Description = "Run execution in a loop."
		};
		rootCommand.Add(loopOption);

		rootCommand.SetAction(async r =>
		{
			var path = r.GetValue(pathOption);
			var repeat = r.GetValue(loopOption);

			try
			{
				do
				{
					await Generate(path).ConfigureAwait(false);

					if (repeat)
					{
						Console.WriteLine("Press any key to run or ESC to cancel...");
						var key = Console.ReadKey();
						if (key.Key == ConsoleKey.Escape)
						{
							return;
						}
					}
				}
				while (repeat);
			}
			catch (Exception ex) when (IsTemplateReadFailure(ex))
			{
				Environment.ExitCode = 1;
			}
			catch (Exception ex)
			{
				Environment.ExitCode = 1;
				Console.Error.WriteLine($"Execution failed: {ex}");
			}
		});

		try
		{
			return await rootCommand.Parse(args).InvokeAsync().ConfigureAwait(false);
		}
		catch (Exception ex) when (IsTemplateReadFailure(ex))
		{
			return 1;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"Execution failed: {ex}");
			return 1;
		}
	}
}