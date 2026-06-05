namespace <# Project.Namespace#>.Service.Shared.Configuration;

public sealed class StorageConfiguration
{
	public StorageProviderKind? Primary { get; set; }
}<#cs SetOutputPathAndSkipOtherDefinitions()#>