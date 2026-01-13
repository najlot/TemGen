using Najlot.Map;
using <#cs Write(Project.Namespace)#>.Client.Data.Mappings;

namespace <#cs Write(Project.Namespace)#>.Client.Data;

public static class MapRegisterExtensions
{
	public static IMap RegisterDataMappings(this IMap map)
	{
<#cs 
foreach (var definition in Definitions)
{
    if (definition.Name.ToLower() == "user")
    {
        continue;
    }
    else if (definition.IsEnumeration)
    {
        continue;
    }

    WriteLine($"		map.Register<{definition.Name}Mappings>();");
}
#>		map.Register<UserMappings>();

		return map;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>