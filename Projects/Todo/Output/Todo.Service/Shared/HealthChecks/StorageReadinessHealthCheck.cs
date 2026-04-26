using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using Todo.Service.Shared.Configuration;
using Todo.Service.Infrastructure.Persistence.LiteDb;
using Todo.Service.Infrastructure.Persistence.MongoDb;
using Todo.Service.Infrastructure.Persistence.MySql;

namespace Todo.Service.Shared.HealthChecks;

public sealed class StorageReadinessHealthCheck(IServiceProvider serviceProvider) : IHealthCheck
{
	public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
	{
		using var scope = serviceProvider.CreateScope();
		var services = scope.ServiceProvider;

		try
		{
			if (services.GetService(typeof(MySqlDbContext)) is MySqlDbContext mySqlDbContext)
			{
				var canConnect = await mySqlDbContext.Database.CanConnectAsync(cancellationToken);
				return canConnect
					? HealthCheckResult.Healthy("MySQL storage is reachable.")
					: HealthCheckResult.Unhealthy("MySQL storage is not reachable.");
			}

			if (services.GetService(typeof(MongoDbContext)) is MongoDbContext mongoDbContext)
			{
				await mongoDbContext.Database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: cancellationToken);
				return HealthCheckResult.Healthy("MongoDB storage is reachable.");
			}

			if (services.GetService(typeof(LiteDbContext)) is LiteDbContext liteDbContext)
			{
				_ = liteDbContext.Database.GetCollectionNames().ToArray();
				return HealthCheckResult.Healthy("LiteDB storage is reachable.");
			}

			if (services.GetService(typeof(FileConfiguration)) is FileConfiguration fileConfiguration)
			{
				var configuredPaths = new[]
				{
					fileConfiguration.FiltersPath,
					fileConfiguration.HistoryPath,
					fileConfiguration.UsersPath,
					fileConfiguration.NotesPath,
					fileConfiguration.TodoItemsPath,
				};

				foreach (var configuredPath in configuredPaths)
				{
					_ = Path.GetFullPath(configuredPath);
				}

				return HealthCheckResult.Healthy("File storage paths are configured.");
			}

			return HealthCheckResult.Healthy("No external storage health probe is configured.");
		}
		catch (Exception ex)
		{
			return HealthCheckResult.Unhealthy("Storage readiness check failed.", ex);
		}
	}
}
