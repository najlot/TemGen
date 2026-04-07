using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using <# Project.Namespace#>.Service.Features.History;
using <# Project.Namespace#>.Service.Features.History.Persistence;
using <# Project.Namespace#>.Service.Features.Users;
using <# Project.Namespace#>.Service.Features.Users.Persistence;
using <# Project.Namespace#>.Service.Infrastructure.Persistence;
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

namespace <# Project.Namespace#>.Service.Infrastructure.Persistence.MySql;

public static class MySqlRepositoryServiceCollectionExtensions
{
	public static IServiceCollection RegisterMySqlRepositories(this IServiceCollection services, MySqlConfiguration configuration)
	{
		return services.RegisterMySqlRepositories(configuration, serviceKey: null);
	}

	public static IServiceCollection RegisterMySqlRepositories(this IServiceCollection services, MySqlConfiguration configuration, object? serviceKey)
	{
		if (serviceKey == null)
		{
			services.AddSingleton(configuration);
			services.AddScoped<MySqlDbContext>();
			services.AddScoped<IUnitOfWork, MySqlUnitOfWork>();
			services.RegisterMySqlHistoryPersistence();
			services.RegisterMySqlUserPersistence();
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			services.RegisterMySql<# definition.Name#>Persistence();
<#end
#>
			return services;
		}

		services.AddKeyedSingleton(serviceKey, configuration);
		services.AddKeyedScoped<MySqlDbContext>(serviceKey, static (serviceProvider, key) =>
			new MySqlDbContext(
				serviceProvider.GetRequiredKeyedService<MySqlConfiguration>(key),
				serviceProvider.GetRequiredService<ILoggerFactory>()));
		services.AddKeyedScoped<IUnitOfWork>(serviceKey, static (serviceProvider, key) =>
			new MySqlUnitOfWork(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		services.RegisterMySqlHistoryPersistence(serviceKey);
		services.RegisterMySqlUserPersistence(serviceKey);
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>		services.RegisterMySql<# definition.Name#>Persistence(serviceKey);
<#end#>

		return services;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>