using Microsoft.Extensions.DependencyInjection;
using <# Project.Namespace#>.Service.Features.Filters;
using <# Project.Namespace#>.Service.Features.Filters.Persistence;
using <# Project.Namespace#>.Service.Features.History;
using <# Project.Namespace#>.Service.Features.History.Persistence;
using <# Project.Namespace#>.Service.Features.Users;
using <# Project.Namespace#>.Service.Features.Users.Persistence;
using <# Project.Namespace#>.Service.Shared.Configuration;
<#cs foreach (var definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))).OrderBy(d => d.Name))
{
	WriteLine($"using {Project.Namespace}.Service.Features.{definition.Name}s;");
	WriteLine($"using {Project.Namespace}.Service.Features.{definition.Name}s.Persistence;");
}
#>

namespace <# Project.Namespace#>.Service.Infrastructure.Persistence.File;

public static class FileRepositoryServiceCollectionExtensions
{
	public static IServiceCollection RegisterFileRepositories(this IServiceCollection services, FileConfiguration configuration)
	{
		return services.RegisterFileRepositories(configuration, serviceKey: null);
	}

	public static IServiceCollection RegisterFileRepositories(this IServiceCollection services, FileConfiguration configuration, object? serviceKey)
	{
		if (serviceKey == null)
		{
			services.AddSingleton(configuration);
			services.AddScoped<IUnitOfWork, FileUnitOfWork>();
			services.RegisterFileFilterPersistence();
			services.RegisterFileHistoryPersistence();
			services.RegisterFileUserPersistence();
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			services.RegisterFile<# definition.Name#>Persistence();
<#end
#>
			return services;
		}

		services.AddKeyedSingleton(serviceKey, configuration);
		services.AddKeyedScoped<IUnitOfWork>(serviceKey, static (_, _) => new FileUnitOfWork());
		services.RegisterFileFilterPersistence(serviceKey);
		services.RegisterFileHistoryPersistence(serviceKey);
		services.RegisterFileUserPersistence(serviceKey);
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>		services.RegisterFile<# definition.Name#>Persistence(serviceKey);
<#end#>

		return services;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>