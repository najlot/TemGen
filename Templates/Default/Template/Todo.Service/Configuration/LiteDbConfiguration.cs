namespace <# Project.Namespace#>.Service.Configuration;

public sealed class LiteDbConfiguration
{
	public string DatabasePath { get; set; } = "Data/<# Project.Namespace#>.db";
}<#cs SetOutputPathAndSkipOtherDefinitions()#>