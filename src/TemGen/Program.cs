using Najlot.Log;
using System;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using TemGen.Handler;

namespace TemGen;

internal class Program
{
	private static async Task Generate(LogAdministrator admin, string path)
	{
		var sw = Stopwatch.StartNew();

		var log = admin.GetLogger("Main");
		log.Info("Starting execution...");

		var project = ProjectReader.ReadProject(path);
		var definitions = DefinitionsReader.ReadDefinitions(project.DefinitionsPath);
		var templates = TemplatesReader.ReadTemplates(project.TemplatesPath);
		var processor = new TemplateProcessor(
			[
				new TextSectionHandler(),
				new CsSectionHandler(),
				new ReflectionSectionHandler(),
				new PySectionHandler(),
				new JintSectionHandler(),
				new LuaSectionHandler()
			], project, definitions);

		if (!string.IsNullOrWhiteSpace(project.ResourcesScriptPath))
		{
			var resourcesScript = TemplatesReader.ReadResourceScript(path, project.ResourcesScriptPath);

			try
			{
				log.Debug("Processing resources script {ScriptPath}...", project.ResourcesScriptPath);
				await processor.Handle(resourcesScript, new Definition() { Entries = [] }, null).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				log.Error(ex, "Error processing script {ScriptPath}: ", project.ResourcesScriptPath);
			}
		}

		await Parallel.ForEachAsync(templates, async (template, tkn) =>
		{
			try
			{
				var results = await processor.Handle(template, definitions).ConfigureAwait(false);

				foreach (var result in results)
				{
					var destPath = Path.Combine(project.OutputPath, result.Key);
					var dirPath = Path.GetDirectoryName(destPath);
					Directory.CreateDirectory(dirPath);

					if (File.Exists(destPath))
					{
						var currentContent = await File.ReadAllTextAsync(destPath, tkn).ConfigureAwait(false);

						if (currentContent == result.Value)
						{
							continue;
						}
					}

					await File.WriteAllTextAsync(destPath, result.Value, template.Encoding, tkn).ConfigureAwait(false);
				}
			}
			catch (Exception ex)
			{
				log.Error(ex, "Error processing {TemplatePath}: ", template.RelativePath);
			}
		}).ConfigureAwait(false);

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

		var pathOption = new Option<string>(
			aliases: ["--path", "-p"],
			description: "Path to a project definition file or a folder containing a ProjectDefinition file.",
			getDefaultValue: () => ".");
		rootCommand.AddOption(pathOption);

		var loopOption = new Option<bool>(
			aliases: ["--loop", "-l"],
			description: "Run execution in a loop.");
		rootCommand.AddOption(loopOption);

		var logLevelOption = new Option<LogLevel>(
			name: "--log-level",
			description: "Log level to use.",
			getDefaultValue: () => LogLevel.Info);
		rootCommand.AddOption(logLevelOption);

		rootCommand.SetHandler(async (path, repeat, logLevel) =>
		{
			admin.SetLogLevel(logLevel);

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
		}, pathOption, loopOption, logLevelOption);

		return await rootCommand.InvokeAsync(args).ConfigureAwait(false);
	}
}