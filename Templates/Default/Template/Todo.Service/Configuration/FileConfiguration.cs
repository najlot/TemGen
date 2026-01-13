namespace <#cs Write(Project.Namespace)#>.Service.Configuration;

public class FileConfiguration
{
	public string UsersPath { get; set; } = "Data/Users";
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"	public string {definition.Name}sPath {{ get; set; }} = \"Data/{definition.Name}s\";");
}
#>}<#cs SetOutputPathAndSkipOtherDefinitions()#>