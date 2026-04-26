using <# Project.Namespace#>.Service.Features.Filters;
using <# Project.Namespace#>.Service.Features.History;
using <# Project.Namespace#>.Service.Features.Users;
using <# Project.Namespace#>.Service.Infrastructure.Persistence;
using <# Project.Namespace#>.Service.Infrastructure.Persistence.LiteDb;
using <# Project.Namespace#>.Service.Infrastructure.Persistence.MySql;
using <# Project.Namespace#>.Service.Shared.Configuration;
<#cs foreach (var definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))).OrderBy(d => d.Name))
{
	WriteLine($"using {Project.Namespace}.Service.Features.{definition.Name}s;");
}
#>

namespace <# Project.Namespace#>.Service.Infrastructure.StorageBackup;

public sealed class StorageBackupHostedService(
	IServiceScopeFactory serviceScopeFactory,
	StartupConfiguration startupConfiguration,
	ILogger<StorageBackupHostedService> logger) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		if (startupConfiguration.BackupConfiguration is not { } backupConfiguration)
		{
			return;
		}

		while (!stoppingToken.IsCancellationRequested)
		{
			var now = DateTime.Now;
			var scheduledLocal = new DateTime(now.Year, now.Month, now.Day,
				backupConfiguration.Hour, backupConfiguration.Minute, 0, DateTimeKind.Local);

			if (scheduledLocal <= now)
			{
				scheduledLocal = scheduledLocal.AddDays(1);
			}

			var delay = scheduledLocal - DateTime.Now;
			if (delay < TimeSpan.Zero)
			{
				delay = TimeSpan.Zero;
			}

			logger.LogInformation(
				"Next storage backup from {Source} to {Target} is scheduled for {ScheduledLocal}.",
				backupConfiguration.Source,
				backupConfiguration.Target,
				scheduledLocal);

			try
			{
				await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}

			try
			{
				await RunBackupAsync(backupConfiguration, stoppingToken).ConfigureAwait(false);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(
					ex,
					"Nightly backup from {Source} to {Target} failed.",
					backupConfiguration.Source,
					backupConfiguration.Target);
			}
		}
	}

	private async Task RunBackupAsync(BackupConfiguration backupConfiguration, CancellationToken cancellationToken)
	{
		ValidateSourceAvailability(backupConfiguration.Source);

		using var scope = serviceScopeFactory.CreateScope();
		var serviceProvider = scope.ServiceProvider;

		EnsureTargetStorageCreated(serviceProvider, backupConfiguration.Target);

		await SynchronizeAsync<FilterModel>(serviceProvider, cancellationToken).ConfigureAwait(false);
		await SynchronizeAsync<HistoryModel>(serviceProvider, cancellationToken).ConfigureAwait(false);
		await SynchronizeAsync<UserModel>(serviceProvider, cancellationToken).ConfigureAwait(false);
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))).OrderBy(d => d.Name)
#>		await SynchronizeAsync<<# definition.Name#>Model>(serviceProvider, cancellationToken).ConfigureAwait(false);
<#end#>
		var unitOfWork = serviceProvider.GetRequiredKeyedService<IUnitOfWork>(BackupRepositoryServiceCollectionExtensions.BackupTargetKey);
		await unitOfWork.CommitAsync().ConfigureAwait(false);

		logger.LogInformation(
			"Nightly backup from {Source} to {Target} completed successfully.",
			backupConfiguration.Source,
			backupConfiguration.Target);
	}

	private void ValidateSourceAvailability(StorageProviderKind storageProvider)
	{
		switch (storageProvider)
		{
			case StorageProviderKind.File:
				var fileConfiguration = startupConfiguration.FileConfiguration
					?? throw new InvalidOperationException("FileConfiguration is required for file backups.");

				var configuredPaths = new[]
				{
					fileConfiguration.FiltersPath,
					fileConfiguration.HistoryPath,
					fileConfiguration.UsersPath,
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))).OrderBy(d => d.Name)
#>					fileConfiguration.<# definition.Name#>sPath,
<#end#>				};

				if (!configuredPaths.Any(path => Directory.Exists(Path.GetFullPath(path))))
				{
					throw new InvalidOperationException("The configured file backup source does not exist.");
				}

				break;

			case StorageProviderKind.LiteDb:
				var liteDbConfiguration = startupConfiguration.LiteDbConfiguration
					?? throw new InvalidOperationException("LiteDbConfiguration is required for LiteDB backups.");

				if (!File.Exists(Path.GetFullPath(liteDbConfiguration.DatabasePath)))
				{
					throw new InvalidOperationException("The configured LiteDB backup source does not exist.");
				}

				break;
		}
	}

	private static void EnsureTargetStorageCreated(IServiceProvider serviceProvider, StorageProviderKind storageProvider)
	{
		switch (storageProvider)
		{
			case StorageProviderKind.LiteDb:
				serviceProvider
					.GetRequiredKeyedService<LiteDbContext>(BackupRepositoryServiceCollectionExtensions.BackupTargetKey)
					.EnsureCreated();
				break;

			case StorageProviderKind.MySql:
				serviceProvider
					.GetRequiredKeyedService<MySqlDbContext>(BackupRepositoryServiceCollectionExtensions.BackupTargetKey)
					.Database
					.EnsureCreated();
				break;
		}
	}

	private static async Task SynchronizeAsync<TModel>(IServiceProvider serviceProvider, CancellationToken cancellationToken)
		where TModel : class, IEntityModel
	{
		cancellationToken.ThrowIfCancellationRequested();

		var sourceRepository = serviceProvider.GetRequiredKeyedService<IEntityRepository<TModel>>(BackupRepositoryServiceCollectionExtensions.BackupSourceKey);
		var targetRepository = serviceProvider.GetRequiredKeyedService<IEntityRepository<TModel>>(BackupRepositoryServiceCollectionExtensions.BackupTargetKey);

		var sourceItems = sourceRepository.GetAllQueryable().ToList();
		var targetItems = targetRepository.GetAllQueryable().ToList();
		var targetIds = targetItems.Select(item => item.Id).ToHashSet();

		foreach (var sourceItem in sourceItems)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (targetIds.Contains(sourceItem.Id))
			{
				await targetRepository.Update(sourceItem).ConfigureAwait(false);
			}
			else
			{
				await targetRepository.Insert(sourceItem).ConfigureAwait(false);
			}
		}

		var sourceIds = sourceItems.Select(item => item.Id).ToHashSet();

		foreach (var targetItem in targetItems)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (!sourceIds.Contains(targetItem.Id))
			{
				await targetRepository.Delete(targetItem.Id).ConfigureAwait(false);
			}
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>