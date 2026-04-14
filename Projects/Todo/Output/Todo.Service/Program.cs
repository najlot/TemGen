using Najlot.Audit;
using Najlot.Log;
using Todo.Service.Shared.Configuration;
using Todo.Service.Infrastructure.Persistence;
using Todo.Service.Infrastructure.Persistence.File;
using Todo.Service.Infrastructure.Persistence.LiteDb;
using Todo.Service.Infrastructure.Persistence.MongoDb;
using Todo.Service.Infrastructure.Persistence.MySql;

namespace Todo.Service;

internal static class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		builder.ConfigureServiceLogging();

		var startupConfiguration = builder.Configuration.LoadStartupConfiguration();

		var services = builder.Services;
		services.AddSingleton(startupConfiguration);
		services.AddSingleton(startupConfiguration.ServiceConfiguration);

		switch (startupConfiguration.PrimaryStorage)
		{
			case StorageProviderKind.MongoDb:
				services.RegisterMongoDbRepositories(startupConfiguration.MongoDbConfiguration ?? throw new InvalidOperationException("MongoDbConfiguration is missing."));
				break;

			case StorageProviderKind.MySql:
				services.RegisterMySqlRepositories(startupConfiguration.MySqlConfiguration ?? throw new InvalidOperationException("MySqlConfiguration is missing."));
				break;

			case StorageProviderKind.LiteDb:
				services.RegisterLiteDbRepositories(startupConfiguration.LiteDbConfiguration ?? throw new InvalidOperationException("LiteDbConfiguration is missing."));
				break;

			case StorageProviderKind.File:
				services.RegisterFileRepositories(startupConfiguration.FileConfiguration ?? throw new InvalidOperationException("FileConfiguration is missing."));
				break;

			default:
				throw new InvalidOperationException("No primary storage configuration is available.");
		}

		services.RegisterBackupRepositories(startupConfiguration);

		var audit = new Audit().RegisterTodoServiceAuditProviders();
		services.AddSingleton(audit);

		var map = new Najlot.Map.Map().RegisterTodoServiceMappings();
		services.AddSingleton(map);

		services.RegisterServices();
		services.RegisterApiInfrastructure(startupConfiguration.ServiceConfiguration);

		var app = builder.Build();
		app.ConfigureApplicationPipeline();

		try
		{
			app.EnsureStorageCreated();
			await app.RunAsync();
		}
		catch (Exception ex)
		{
			var log = LogAdministrator.Instance.GetLogger(typeof(Program));
			log.Error(ex, "An unhandled exception occurred while running the application.");
			LogAdministrator.Instance.Flush();
		}

		LogAdministrator.Instance.Dispose();
	}
}
