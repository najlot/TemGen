using Microsoft.Extensions.DependencyInjection;

namespace <#cs Write(Project.Namespace)#>.Service.Mappings;

public static class ServiceCollectionExtensions
{
	public static Najlot.Map.IMap RegisterDataMappings(this Najlot.Map.IMap map)
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

	public static IServiceCollection RegisterDataMappings(this IServiceCollection services)
	{
		var map = new Najlot.Map.Map().RegisterDataMappings();
		return services.AddSingleton(map);
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>