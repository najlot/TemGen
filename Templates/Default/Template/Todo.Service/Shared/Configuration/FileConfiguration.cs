namespace <# Project.Namespace#>.Service.Shared.Configuration;

public class FileConfiguration
{
	public string FiltersPath { get; set; } = "Data/Filters";
	public string UsersPath { get; set; } = "Data/Users";
	public string HistoryPath { get; set; } = "Data/History";
<#for definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>	public string <# definition.Name#>sPath { get; set; } = "Data/<# definition.Name#>s";
<#end#>}<#cs SetOutputPathAndSkipOtherDefinitions()#>