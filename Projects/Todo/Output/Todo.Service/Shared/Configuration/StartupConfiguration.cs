namespace Todo.Service.Shared.Configuration;

public sealed class StartupConfiguration
{
	public required StorageProviderKind PrimaryStorage { get; init; }

	public FileConfiguration? FileConfiguration { get; init; }

	public LiteDbConfiguration? LiteDbConfiguration { get; init; }

	public MongoDbConfiguration? MongoDbConfiguration { get; init; }

	public MySqlConfiguration? MySqlConfiguration { get; init; }

	public BackupConfiguration? BackupConfiguration { get; init; }

	public required ServiceConfiguration ServiceConfiguration { get; init; }
}