using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using <# Project.Namespace#>.Service.Configuration;
using <# Project.Namespace#>.Service.Model;

namespace <# Project.Namespace#>.Service.Repository.MySqlImpl;

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
			services.AddScoped<IUserRepository, MySqlUserRepository>();
			services.AddScoped<IEntityRepository<UserModel>, MySqlUserRepository>();
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			services.AddScoped<I<# definition.Name#>Repository, MySql<# definition.Name#>Repository>();
			services.AddScoped<IEntityRepository<<# definition.Name#>Model>, MySql<# definition.Name#>Repository>();
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
		services.AddKeyedScoped<IUserRepository>(serviceKey, static (serviceProvider, key) =>
			new MySqlUserRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<UserModel>>(serviceKey, static (serviceProvider, key) =>
			new MySqlUserRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>		services.AddKeyedScoped<I<# definition.Name#>Repository>(serviceKey, static (serviceProvider, key) =>
			new MySql<# definition.Name#>Repository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<<# definition.Name#>Model>>(serviceKey, static (serviceProvider, key) =>
			new MySql<# definition.Name#>Repository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
<#end#>

		return services;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>