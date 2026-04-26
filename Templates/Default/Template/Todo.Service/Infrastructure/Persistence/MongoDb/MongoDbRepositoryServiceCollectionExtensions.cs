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

namespace <# Project.Namespace#>.Service.Infrastructure.Persistence.MongoDb;

public static class MongoDbRepositoryServiceCollectionExtensions
{
	public static IServiceCollection RegisterMongoDbRepositories(this IServiceCollection services, MongoDbConfiguration configuration)
	{
		return services.RegisterMongoDbRepositories(configuration, serviceKey: null);
	}

	public static IServiceCollection RegisterMongoDbRepositories(this IServiceCollection services, MongoDbConfiguration configuration, object? serviceKey)
	{
		if (serviceKey == null)
		{
			services.AddSingleton(configuration);
			services.AddSingleton<MongoDbContext>();
			services.AddScoped<IUnitOfWork, MongoDbUnitOfWork>();
			services.RegisterMongoDbFilterPersistence();
			services.RegisterMongoDbHistoryPersistence();
			services.RegisterMongoDbUserPersistence();
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			services.RegisterMongoDb<# definition.Name#>Persistence();
<#end
#>
			return services;
		}

		services.AddKeyedSingleton(serviceKey, configuration);
		services.AddKeyedSingleton<MongoDbContext>(serviceKey, static (serviceProvider, key) =>
			new MongoDbContext(serviceProvider.GetRequiredKeyedService<MongoDbConfiguration>(key)));
		services.AddKeyedScoped<IUnitOfWork>(serviceKey, static (_, _) => new MongoDbUnitOfWork());
		services.RegisterMongoDbFilterPersistence(serviceKey);
		services.RegisterMongoDbHistoryPersistence(serviceKey);
		services.RegisterMongoDbUserPersistence(serviceKey);
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>		services.RegisterMongoDb<# definition.Name#>Persistence(serviceKey);
<#end#>

		return services;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>