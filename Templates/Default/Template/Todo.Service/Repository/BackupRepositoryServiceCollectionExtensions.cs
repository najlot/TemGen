using Microsoft.Extensions.DependencyInjection;
using <# Project.Namespace#>.Service.Configuration;
using <# Project.Namespace#>.Service.Repository.FileImpl;
using <# Project.Namespace#>.Service.Repository.LiteDbImpl;
using <# Project.Namespace#>.Service.Repository.MongoDbImpl;
using <# Project.Namespace#>.Service.Repository.MySqlImpl;

namespace <# Project.Namespace#>.Service.Repository;

public static class BackupRepositoryServiceCollectionExtensions
{
	public const string BackupSourceKey = "backup-source";
	public const string BackupTargetKey = "backup-target";

	public static IServiceCollection RegisterBackupRepositories(this IServiceCollection services, StartupConfiguration startupConfiguration)
	{
		if (startupConfiguration.BackupConfiguration is not { } backupConfiguration)
		{
			return services;
		}

		services.RegisterRepositories(startupConfiguration, backupConfiguration.Source, BackupSourceKey);
		services.RegisterRepositories(startupConfiguration, backupConfiguration.Target, BackupTargetKey);

		return services;
	}

	private static IServiceCollection RegisterRepositories(
		this IServiceCollection services,
		StartupConfiguration startupConfiguration,
		StorageProviderKind storageProvider,
		string serviceKey)
	{
		switch (storageProvider)
		{
			case StorageProviderKind.File:
				return services.RegisterFileRepositories(
					startupConfiguration.FileConfiguration ?? throw new InvalidOperationException("FileConfiguration is required for backup storage registration."),
					serviceKey);

			case StorageProviderKind.LiteDb:
				return services.RegisterLiteDbRepositories(
					startupConfiguration.LiteDbConfiguration ?? throw new InvalidOperationException("LiteDbConfiguration is required for backup storage registration."),
					serviceKey);

			case StorageProviderKind.MongoDb:
				return services.RegisterMongoDbRepositories(
					startupConfiguration.MongoDbConfiguration ?? throw new InvalidOperationException("MongoDbConfiguration is required for backup storage registration."),
					serviceKey);

			case StorageProviderKind.MySql:
				return services.RegisterMySqlRepositories(
					startupConfiguration.MySqlConfiguration ?? throw new InvalidOperationException("MySqlConfiguration is required for backup storage registration."),
					serviceKey);

			default:
				throw new ArgumentOutOfRangeException(nameof(storageProvider), storageProvider, "Unsupported storage provider.");
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>