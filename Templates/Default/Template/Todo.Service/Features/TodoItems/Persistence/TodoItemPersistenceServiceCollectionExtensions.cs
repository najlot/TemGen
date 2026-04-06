using Microsoft.Extensions.DependencyInjection;
using <# Project.Namespace#>.Service.Infrastructure.Persistence;
using <# Project.Namespace#>.Service.Infrastructure.Persistence.File;
using <# Project.Namespace#>.Service.Infrastructure.Persistence.LiteDb;
using <# Project.Namespace#>.Service.Infrastructure.Persistence.MongoDb;
using <# Project.Namespace#>.Service.Infrastructure.Persistence.MySql;
using <# Project.Namespace#>.Service.Shared.Configuration;

namespace <# Project.Namespace#>.Service.Features.<# Definition.Name#>s.Persistence;

public static class <# Definition.Name#>PersistenceServiceCollectionExtensions
{
	public static IServiceCollection RegisterFile<# Definition.Name#>Persistence(this IServiceCollection services)
	{
		services.AddScoped<I<# Definition.Name#>Repository, File<# Definition.Name#>Repository>();
		services.AddScoped<IEntityRepository<<# Definition.Name#>Model>, File<# Definition.Name#>Repository>();
		return services;
	}

	public static IServiceCollection RegisterFile<# Definition.Name#>Persistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<I<# Definition.Name#>Repository>(serviceKey, static (serviceProvider, key) =>
			new File<# Definition.Name#>Repository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		services.AddKeyedScoped<IEntityRepository<<# Definition.Name#>Model>>(serviceKey, static (serviceProvider, key) =>
			new File<# Definition.Name#>Repository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		return services;
	}

	public static IServiceCollection RegisterLiteDb<# Definition.Name#>Persistence(this IServiceCollection services)
	{
		services.AddScoped<I<# Definition.Name#>Repository, LiteDb<# Definition.Name#>Repository>();
		services.AddScoped<IEntityRepository<<# Definition.Name#>Model>, LiteDb<# Definition.Name#>Repository>();
		return services;
	}

	public static IServiceCollection RegisterLiteDb<# Definition.Name#>Persistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<I<# Definition.Name#>Repository>(serviceKey, static (serviceProvider, key) =>
			new LiteDb<# Definition.Name#>Repository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<<# Definition.Name#>Model>>(serviceKey, static (serviceProvider, key) =>
			new LiteDb<# Definition.Name#>Repository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		return services;
	}

	public static IServiceCollection RegisterMongoDb<# Definition.Name#>Persistence(this IServiceCollection services)
	{
		services.AddScoped<I<# Definition.Name#>Repository, MongoDb<# Definition.Name#>Repository>();
		services.AddScoped<IEntityRepository<<# Definition.Name#>Model>, MongoDb<# Definition.Name#>Repository>();
		return services;
	}

	public static IServiceCollection RegisterMongoDb<# Definition.Name#>Persistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<I<# Definition.Name#>Repository>(serviceKey, static (serviceProvider, key) =>
			new MongoDb<# Definition.Name#>Repository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<<# Definition.Name#>Model>>(serviceKey, static (serviceProvider, key) =>
			new MongoDb<# Definition.Name#>Repository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		return services;
	}

	public static IServiceCollection RegisterMySql<# Definition.Name#>Persistence(this IServiceCollection services)
	{
		services.AddScoped<I<# Definition.Name#>Repository, MySql<# Definition.Name#>Repository>();
		services.AddScoped<IEntityRepository<<# Definition.Name#>Model>, MySql<# Definition.Name#>Repository>();
		return services;
	}

	public static IServiceCollection RegisterMySql<# Definition.Name#>Persistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<I<# Definition.Name#>Repository>(serviceKey, static (serviceProvider, key) =>
			new MySql<# Definition.Name#>Repository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<<# Definition.Name#>Model>>(serviceKey, static (serviceProvider, key) =>
			new MySql<# Definition.Name#>Repository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		return services;
	}
}
<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>