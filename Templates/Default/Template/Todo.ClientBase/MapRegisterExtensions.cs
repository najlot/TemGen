using Najlot.Map;
using <#cs Write(Project.Namespace)#>.ClientBase.Mappings;

namespace <#cs Write(Project.Namespace)#>.ClientBase;

public static class MapRegisterExtensions
{
	public static IMap RegisterViewModelMappings(this IMap map)
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

    WriteLine($"		map.Register<{definition.Name}ViewModelMappings>();");
}
#>
		return map;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>