namespace Todo.Service.Shared.Configuration;

public static class StartupConfigurationExtensions
{
	public static StartupConfiguration LoadStartupConfiguration(this IConfiguration configuration)
	{
		var storageConfigFound = configuration.TryReadConfiguration<StorageConfiguration>(out var storageConfig);
		var backupConfigFound = configuration.TryReadConfiguration<BackupConfiguration>(out var backupConfig);
		var fileConfigFound = configuration.TryReadConfiguration<FileConfiguration>(out var fileConfig);
		var liteDbConfigFound = configuration.TryReadConfiguration<LiteDbConfiguration>(out var liteDbConfig);
		var mongoDbConfigFound = configuration.TryReadConfiguration<MongoDbConfiguration>(out var mongoDbConfig);
		var mysqlConfigFound = configuration.TryReadConfiguration<MySqlConfiguration>(out var mysqlConfig);
		var serviceConfigFound = configuration.TryReadConfiguration<ServiceConfiguration>(out var serviceConfig);

		if (!(fileConfigFound || liteDbConfigFound || mongoDbConfigFound || mysqlConfigFound))
		{
			ConfigurationReader.WriteConfigurationExample<StorageConfiguration>();
			ConfigurationReader.WriteConfigurationExample<BackupConfiguration>();
			ConfigurationReader.WriteConfigurationExample<FileConfiguration>();
			ConfigurationReader.WriteConfigurationExample<LiteDbConfiguration>();
			ConfigurationReader.WriteConfigurationExample<MongoDbConfiguration>();
			ConfigurationReader.WriteConfigurationExample<MySqlConfiguration>();
		}

		if (!serviceConfigFound)
		{
			ConfigurationReader.WriteConfigurationExample<ServiceConfiguration>();
		}

		if (string.IsNullOrWhiteSpace(serviceConfig?.Secret))
		{
			throw new Exception($"Please set {nameof(ServiceConfiguration.Secret)} in the {nameof(ServiceConfiguration)}!");
		}

		var primaryStorage = ResolvePrimaryStorage(
			storageConfigFound ? storageConfig : null,
			fileConfigFound,
			liteDbConfigFound,
			mongoDbConfigFound,
			mysqlConfigFound);

		ValidateBackupConfiguration(
			backupConfigFound ? backupConfig : null,
			fileConfigFound,
			liteDbConfigFound,
			mongoDbConfigFound,
			mysqlConfigFound);

		return new StartupConfiguration
		{
			PrimaryStorage = primaryStorage,
			FileConfiguration = fileConfig,
			LiteDbConfiguration = liteDbConfig,
			MongoDbConfiguration = mongoDbConfig,
			MySqlConfiguration = mysqlConfig,
			BackupConfiguration = backupConfigFound ? backupConfig : null,
			ServiceConfiguration = serviceConfig,
		};
	}

	private static StorageProviderKind ResolvePrimaryStorage(
		StorageConfiguration? storageConfiguration,
		bool fileConfigFound,
		bool liteDbConfigFound,
		bool mongoDbConfigFound,
		bool mySqlConfigFound)
	{
		if (storageConfiguration?.Primary is { } configuredPrimary)
		{
			ValidateStorageAvailability(configuredPrimary, fileConfigFound, liteDbConfigFound, mongoDbConfigFound, mySqlConfigFound);
			return configuredPrimary;
		}

		if (mongoDbConfigFound)
		{
			return StorageProviderKind.MongoDb;
		}

		if (mySqlConfigFound)
		{
			return StorageProviderKind.MySql;
		}

		if (liteDbConfigFound)
		{
			return StorageProviderKind.LiteDb;
		}

		if (fileConfigFound)
		{
			return StorageProviderKind.File;
		}

		throw new InvalidOperationException("At least one storage configuration must be provided.");
	}

	private static void ValidateBackupConfiguration(
		BackupConfiguration? backupConfiguration,
		bool fileConfigFound,
		bool liteDbConfigFound,
		bool mongoDbConfigFound,
		bool mySqlConfigFound)
	{
		if (backupConfiguration is null)
		{
			return;
		}

		if (backupConfiguration.Source == backupConfiguration.Target)
		{
			throw new InvalidOperationException("Backup source and target must use different storage providers.");
		}

		if (backupConfiguration.Hour is < 0 or > 23)
		{
			throw new InvalidOperationException("BackupConfiguration.Hour must be between 0 and 23.");
		}

		if (backupConfiguration.Minute is < 0 or > 59)
		{
			throw new InvalidOperationException("BackupConfiguration.Minute must be between 0 and 59.");
		}

		ValidateStorageAvailability(backupConfiguration.Source, fileConfigFound, liteDbConfigFound, mongoDbConfigFound, mySqlConfigFound);
		ValidateStorageAvailability(backupConfiguration.Target, fileConfigFound, liteDbConfigFound, mongoDbConfigFound, mySqlConfigFound);
	}

	private static void ValidateStorageAvailability(
		StorageProviderKind storageProvider,
		bool fileConfigFound,
		bool liteDbConfigFound,
		bool mongoDbConfigFound,
		bool mySqlConfigFound)
	{
		var isAvailable = storageProvider switch
		{
			StorageProviderKind.File => fileConfigFound,
			StorageProviderKind.LiteDb => liteDbConfigFound,
			StorageProviderKind.MongoDb => mongoDbConfigFound,
			StorageProviderKind.MySql => mySqlConfigFound,
			_ => false,
		};

		if (!isAvailable)
		{
			throw new InvalidOperationException($"{storageProvider} storage was selected but its configuration is missing.");
		}
	}
}