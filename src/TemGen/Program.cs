﻿using Najlot.Log;
using Najlot.Log.Configuration.FileSource;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;

namespace TemGen;

internal class Program
{
	private static async Task Generate(Logger log, string path)
	{
		var sw = Stopwatch.StartNew();

		log.Info("Starting execution...");

		var project = ProjectReader.ReadProject(path);
		var definitions = DefinitionsReader.ReadDefinitions(project.DefinitionsPath);
		var templates = TemplatesReader.ReadTemplates(project.TemplatesPath);
		var processor = new TemplateProcessor(project, definitions);

		if (!string.IsNullOrWhiteSpace(project.ResourcesScriptPath))
		{
			var resourcesScript = TemplatesReader.ReadResourceScript(project.ResourcesScriptPath);

			try
			{
				log.Debug("Processing resources script {ScriptPath}...", project.ResourcesScriptPath);
				await processor.Handle(resourcesScript, new Definition() { Entries = new List<DefinitionEntry>() }, null).ConfigureAwait(false);
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
				foreach (var definition in definitions)
				{
					var result = await processor.Handle(template, definition, null).ConfigureAwait(false);

					if (!string.IsNullOrWhiteSpace(result.RelativePath))
					{
						log.Debug("Writing {RelativePath}", result.RelativePath);
						var destPath = Path.Combine(project.OutputPath, result.RelativePath);
						var dirPath = Path.GetDirectoryName(destPath);
						Directory.CreateDirectory(dirPath);
						await File.WriteAllTextAsync(destPath, result.Content, template.Encoding, tkn).ConfigureAwait(false);
					}

					if (result.SkipOtherDefinitions)
					{
						log.Debug("Skipping other definitions...");
						break;
					}

					if (result.RepeatForEachDefinitionEntry)
					{
						foreach (var entry in definition.Entries)
						{
							result = await processor.Handle(template, definition, entry).ConfigureAwait(false);

							if (!string.IsNullOrWhiteSpace(result.RelativePath))
							{
								log.Debug("Writing {RelativePath}", result.RelativePath);
								var destPath = Path.Combine(project.OutputPath, result.RelativePath);
								var dirPath = Path.GetDirectoryName(destPath);
								Directory.CreateDirectory(dirPath);
								await File.WriteAllTextAsync(destPath, result.Content, template.Encoding, tkn).ConfigureAwait(false);
							}
						}
					}
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

		ProfileOptimization.SetProfileRoot(appdataDir);
		ProfileOptimization.StartProfile("Main");

		using var admin = LogAdministrator.Instance
			.AddConsoleLogDestination(true)
			.SetExecutionMiddleware<Najlot.Log.Middleware.QueueExecutionMiddleware>();

		var log = admin.GetLogger("Main");

		var rootCommand = new RootCommand("TemGen - Template based code generator");

		var pathOption = new Option<string>(
			aliases: new[] { "--path", "-p" },
			description: "Path to a project definition file or a folder containing a ProjectDefinition file.",
			getDefaultValue: () => ".");
		rootCommand.AddOption(pathOption);

		var loopOption = new Option<bool>(
			aliases: new[] { "--loop", "-l" },
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
				await Generate(log, path).ConfigureAwait(false);
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