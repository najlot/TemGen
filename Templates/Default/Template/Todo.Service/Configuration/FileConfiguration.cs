namespace <# Project.Namespace#>.Service.Configuration;

public class FileConfiguration
{
	public string UsersPath { get; set; } = "Data/Users";
<#for definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>	public string <# definition.Name#>sPath { get; set; } = "Data/<# definition.Name#>s";
<#end#>}<#cs SetOutputPathAndSkipOtherDefinitions()#>