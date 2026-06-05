namespace <# Project.Namespace#>.Service.Shared.Configuration;

public sealed class LiteDbConfiguration
{
	public string DatabasePath { get; set; } = "Data/<# Project.Namespace#>.db";
}<#cs SetOutputPathAndSkipOtherDefinitions()#>