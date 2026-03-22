using Microsoft.Extensions.DependencyInjection;
using <# Project.Namespace#>.Service.Configuration;
using <# Project.Namespace#>.Service.Model;

namespace <# Project.Namespace#>.Service.Repository.LiteDbImpl;

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
			services.AddScoped<IUserRepository, LiteDbUserRepository>();
			services.AddScoped<IEntityRepository<UserModel>, LiteDbUserRepository>();
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			services.AddScoped<I<# definition.Name#>Repository, LiteDb<# definition.Name#>Repository>();
			services.AddScoped<IEntityRepository<<# definition.Name#>Model>, LiteDb<# definition.Name#>Repository>();
<#end
#>
			return services;
		}

		services.AddKeyedSingleton(serviceKey, configuration);
		services.AddKeyedSingleton<LiteDbContext>(serviceKey, static (serviceProvider, key) =>
			new LiteDbContext(serviceProvider.GetRequiredKeyedService<LiteDbConfiguration>(key)));
		services.AddKeyedScoped<IUnitOfWork>(serviceKey, static (serviceProvider, key) =>
			new LiteDbUnitOfWork(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.AddKeyedScoped<IUserRepository>(serviceKey, static (serviceProvider, key) =>
			new LiteDbUserRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<UserModel>>(serviceKey, static (serviceProvider, key) =>
			new LiteDbUserRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>		services.AddKeyedScoped<I<# definition.Name#>Repository>(serviceKey, static (serviceProvider, key) =>
			new LiteDb<# definition.Name#>Repository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<<# definition.Name#>Model>>(serviceKey, static (serviceProvider, key) =>
			new LiteDb<# definition.Name#>Repository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
<#end#>

		return services;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>