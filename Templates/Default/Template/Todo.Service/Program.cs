using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Najlot.Log;
using Najlot.Log.Configuration.FileSource;
using Najlot.Log.Destinations;
using Najlot.Log.Extensions.Logging;
using Najlot.Log.Middleware;
using System;
using System.IO;
using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.Service;

internal static class Program
{
	private static void LogErrorOccured(object? sender, LogErrorEventArgs e)
	{
		Console.WriteLine(e.Message + Environment.NewLine + e.Exception);
	}

	public static async Task Main(string[] args)
	{
		var configPath = Path.Combine("config", "Log.config");
		configPath = Path.GetFullPath(configPath);

		LogErrorHandler.Instance.ErrorOccured += LogErrorOccured;

		LogAdministrator.Instance
			.SetLogLevel(Najlot.Log.LogLevel.Debug)
			.SetCollectMiddleware<ConcurrentCollectMiddleware, FileDestination>()
			.SetCollectMiddleware<ConcurrentCollectMiddleware, ConsoleDestination>()
			.AddConsoleDestination(useColors: true)
			.AddFileDestination(
				Path.Combine("logs", "log.txt"),
				30,
				Path.Combine("logs", ".logs"),
				true)
			.ReadConfigurationFromXmlFile(configPath, true, true);

		await CreateWebHostBuilder(args).Build().RunAsync();

		LogAdministrator.Instance.Dispose();
		LogErrorHandler.Instance.ErrorOccured -= LogErrorOccured;
	}

	public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
		WebHost.CreateDefaultBuilder(args)
			.ConfigureLogging(builder =>
			{
				builder.ClearProviders();
				builder.AddNajlotLog(LogAdministrator.Instance);
			})
			.UseStartup<Startup>();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>