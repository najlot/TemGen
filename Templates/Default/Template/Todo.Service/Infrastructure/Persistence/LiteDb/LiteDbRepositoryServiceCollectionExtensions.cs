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

namespace <# Project.Namespace#>.Service.Infrastructure.Persistence.LiteDb;

public static class LiteDbRepositoryServiceCollectionExtensions
{
	public static IServiceCollection RegisterLiteDbRepositories(this IServiceCollection services, LiteDbConfiguration configuration)
	{
		return services.RegisterLiteDbRepositories(configuration, serviceKey: null);
	}

	public static IServiceCollection RegisterLiteDbRepositories(this IServiceCollection services, LiteDbConfiguration configuration, object? serviceKey)
	{
		if (serviceKey == null)
		{
			services.AddSingleton(configuration);
			services.AddSingleton<LiteDbContext>();
			services.AddScoped<IUnitOfWork, LiteDbUnitOfWork>();
			services.RegisterLiteDbFilterPersistence();
			services.RegisterLiteDbHistoryPersistence();
			services.RegisterLiteDbUserPersistence();
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			services.RegisterLiteDb<# definition.Name#>Persistence();
<#end
#>
			return services;
		}

		services.AddKeyedSingleton(serviceKey, configuration);
		services.AddKeyedSingleton<LiteDbContext>(serviceKey, static (serviceProvider, key) =>
			new LiteDbContext(serviceProvider.GetRequiredKeyedService<LiteDbConfiguration>(key)));
		services.AddKeyedScoped<IUnitOfWork>(serviceKey, static (serviceProvider, key) =>
			new LiteDbUnitOfWork(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.RegisterLiteDbFilterPersistence(serviceKey);
		services.RegisterLiteDbHistoryPersistence(serviceKey);
		services.RegisterLiteDbUserPersistence(serviceKey);
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>		services.RegisterLiteDb<# definition.Name#>Persistence(serviceKey);
<#end#>

		return services;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>