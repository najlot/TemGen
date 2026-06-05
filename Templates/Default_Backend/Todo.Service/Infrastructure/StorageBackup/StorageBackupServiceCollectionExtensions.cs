using Microsoft.Extensions.DependencyInjection;

namespace <# Project.Namespace#>.Service.Infrastructure.StorageBackup;

public static class StorageBackupServiceCollectionExtensions
{
	public static IServiceCollection RegisterStorageBackupInfrastructure(this IServiceCollection services)
	{
		services.AddHostedService<StorageBackupHostedService>();
		return services;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>