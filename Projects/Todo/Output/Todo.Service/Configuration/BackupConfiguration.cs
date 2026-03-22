namespace Todo.Service.Configuration;

public sealed class BackupConfiguration
{
	public StorageProviderKind Source { get; set; } = StorageProviderKind.LiteDb;

	public StorageProviderKind Target { get; set; } = StorageProviderKind.File;

	public int Hour { get; set; } = 2;

	public int Minute { get; set; }
}