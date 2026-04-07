using Najlot.Log;
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

	private static async Task Generate(LogAdministrator admin, string path)
	{
		var sw = Stopwatch.StartNew();

		var log = admin.GetLogger("Main");
		log.Info("Starting execution...");

		var project = ProjectReader.ReadProject(path);
		var definitions = DefinitionsReader.ReadDefinitions(project.DefinitionsPath);
		
		var csScripts = Array.Empty<string>();
		var jsScripts = Array.Empty<string>();
		var pyScripts = Array.Empty<string>();
		var luaScripts = Array.Empty<string>();

		if (!string.IsNullOrWhiteSpace(project.ScriptsPath))
		{
			var scripts = TemplatesReader.ReadScripts(project.ScriptsPath);
			csScripts = scripts.Where(script => script.Sections[0].Handler == TemplateHandler.CSharp).Select(s => s.Sections[0].Content).ToArray();
			jsScripts = scripts.Where(script => script.Sections[0].Handler == TemplateHandler.JavaScript).Select(s => s.Sections[0].Content).ToArray();
			pyScripts = scripts.Where(script => script.Sections[0].Handler == TemplateHandler.Python).Select(s => s.Sections[0].Content).ToArray();
			luaScripts = scripts.Where(script => script.Sections[0].Handler == TemplateHandler.Lua).Select(s => s.Sections[0].Content).ToArray();
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
			log.Error(ex, "Could not read generation manifest {ManifestPath}. Stale-file cleanup will be skipped for this run.", GeneratedOutputManifest.GetManifestPath(project.OutputPath));
		}

		if (!string.IsNullOrWhiteSpace(project.ResourcesScriptPath))
		{
			var resourcesScript = TemplatesReader.ReadScript(path, project.ResourcesScriptPath);

			try
			{
				log.Debug("Processing resources script {ScriptPath}...", project.ResourcesScriptPath);
				await processor.Handle(resourcesScript, new Definition() { Entries = [] }, null).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				hasProcessingErrors = true;
				log.Error(ex, "Error processing script {ScriptPath}: ", project.ResourcesScriptPath);
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
					log.Error("Error reading template {TemplatePath}: {Message}", error.TemplatePath, error.InnerException?.Message ?? error.Message);
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
					log.Error(ex, "Error processing {TemplatePath}: ", template.RelativePath);
				}
			}).ConfigureAwait(false);
		}

		if (hasProcessingErrors)
		{
			log.Info("Skipping stale-file cleanup because generation completed with errors.");
		}
		else
		{
			var currentManifest = new GeneratedOutputManifest
			{
				Files = generatedFiles.OrderBy(f => f, StringComparer.Ordinal).ToHashSet(StringComparer.Ordinal)
			};

			var cleanupResult = await GeneratedOutputCleaner
				.CleanupAsync(project.OutputPath, previousManifest, currentManifest)
				.ConfigureAwait(false);

			if (cleanupResult.DeletedCount > 0)
			{
				log.Info("Stale-file cleanup removed {DeletedCount} file(s).", cleanupResult.DeletedCount);
			}

			await currentManifest.SaveAsync(project.OutputPath).ConfigureAwait(false);
		}
		
		log.Info("Done after {Elapsed}.", sw.Elapsed);
	}

	private static async Task<int> Main(string[] args)
	{
		var appdataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		appdataDir = Path.Combine(appdataDir, typeof(Program).Namespace);
		Directory.CreateDirectory(appdataDir);

		using var admin = LogAdministrator.Instance
			.AddConsoleDestination(true)
			.SetCollectMiddleware<Najlot.Log.Middleware.ConcurrentCollectMiddleware, Najlot.Log.Destinations.ConsoleDestination>();

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

		var logLevelOption = new Option<LogLevel>("--log-level")
		{
			Description = "Log level to use.",
			DefaultValueFactory = (r) => LogLevel.Info
		};
		rootCommand.Add(logLevelOption);

		rootCommand.SetAction(async r =>
		{
			var path = r.GetValue(pathOption);
			var repeat = r.GetValue(loopOption);
			var logLevel = r.GetValue(logLevelOption);

			admin.SetLogLevel(logLevel);

			try
			{
				do
				{
					await Generate(admin, path).ConfigureAwait(false);
					admin.Flush();

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
				admin.Flush();
			}
			catch (Exception ex)
			{
				Environment.ExitCode = 1;
				admin.GetLogger("Main").Error(ex, "Execution failed.");
				admin.Flush();
			}
		});

		try
		{
			return await rootCommand.Parse(args).InvokeAsync().ConfigureAwait(false);
		}
		catch (Exception ex) when (IsTemplateReadFailure(ex))
		{
			admin.Flush();
			return 1;
		}
		catch (Exception ex)
		{
			admin.GetLogger("Main").Error(ex, "Execution failed.");
			admin.Flush();
			return 1;
		}
	}
}