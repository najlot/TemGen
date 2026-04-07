using Microsoft.Extensions.DependencyInjection;

namespace Todo.Service.Infrastructure.StorageBackup;

public static class StorageBackupServiceCollectionExtensions
{
	public static IServiceCollection RegisterStorageBackupInfrastructure(this IServiceCollection services)
	{
		services.AddHostedService<StorageBackupHostedService>();
		return services;
	}
}
