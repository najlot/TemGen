using Microsoft.Extensions.DependencyInjection;
using <# Project.Namespace#>.Service.Configuration;
using <# Project.Namespace#>.Service.Model;

namespace <# Project.Namespace#>.Service.Repository.MongoDbImpl;

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
			services.AddScoped<IUserRepository, MongoDbUserRepository>();
			services.AddScoped<IEntityRepository<UserModel>, MongoDbUserRepository>();
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			services.AddScoped<I<# definition.Name#>Repository, MongoDb<# definition.Name#>Repository>();
			services.AddScoped<IEntityRepository<<# definition.Name#>Model>, MongoDb<# definition.Name#>Repository>();
<#end
#>
			return services;
		}

		services.AddKeyedSingleton(serviceKey, configuration);
		services.AddKeyedSingleton<MongoDbContext>(serviceKey, static (serviceProvider, key) =>
			new MongoDbContext(serviceProvider.GetRequiredKeyedService<MongoDbConfiguration>(key)));
		services.AddKeyedScoped<IUnitOfWork>(serviceKey, static (_, _) => new MongoDbUnitOfWork());
		services.AddKeyedScoped<IUserRepository>(serviceKey, static (serviceProvider, key) =>
			new MongoDbUserRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<UserModel>>(serviceKey, static (serviceProvider, key) =>
			new MongoDbUserRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>		services.AddKeyedScoped<I<# definition.Name#>Repository>(serviceKey, static (serviceProvider, key) =>
			new MongoDb<# definition.Name#>Repository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<<# definition.Name#>Model>>(serviceKey, static (serviceProvider, key) =>
			new MongoDb<# definition.Name#>Repository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
<#end#>

		return services;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>