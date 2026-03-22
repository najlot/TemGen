using Microsoft.Extensions.DependencyInjection;
using <# Project.Namespace#>.Service.Configuration;
using <# Project.Namespace#>.Service.Model;

namespace <# Project.Namespace#>.Service.Repository.FileImpl;

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
			services.AddScoped<IUserRepository, FileUserRepository>();
			services.AddScoped<IEntityRepository<UserModel>, FileUserRepository>();
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			services.AddScoped<I<# definition.Name#>Repository, File<# definition.Name#>Repository>();
			services.AddScoped<IEntityRepository<<# definition.Name#>Model>, File<# definition.Name#>Repository>();
<#end
#>
			return services;
		}

		services.AddKeyedSingleton(serviceKey, configuration);
		services.AddKeyedScoped<IUnitOfWork>(serviceKey, static (_, _) => new FileUnitOfWork());
		services.AddKeyedScoped<IUserRepository>(serviceKey, static (serviceProvider, key) =>
			new FileUserRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		services.AddKeyedScoped<IEntityRepository<UserModel>>(serviceKey, static (serviceProvider, key) =>
			new FileUserRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>		services.AddKeyedScoped<I<# definition.Name#>Repository>(serviceKey, static (serviceProvider, key) =>
			new File<# definition.Name#>Repository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		services.AddKeyedScoped<IEntityRepository<<# definition.Name#>Model>>(serviceKey, static (serviceProvider, key) =>
			new File<# definition.Name#>Repository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
<#end#>

		return services;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>