using Microsoft.Extensions.DependencyInjection;
using Todo.Service.Infrastructure.Persistence;
using Todo.Service.Infrastructure.Persistence.File;
using Todo.Service.Infrastructure.Persistence.LiteDb;
using Todo.Service.Infrastructure.Persistence.MongoDb;
using Todo.Service.Infrastructure.Persistence.MySql;
using Todo.Service.Shared.Configuration;

namespace Todo.Service.Features.Filters.Persistence;

public static class FilterPersistenceServiceCollectionExtensions
{
	public static IServiceCollection RegisterFileFilterPersistence(this IServiceCollection services)
	{
		services.AddScoped<IFilterRepository, FileFilterRepository>();
		services.AddScoped<IEntityRepository<FilterModel>, FileFilterRepository>();
		return services;
	}

	public static IServiceCollection RegisterFileFilterPersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<IFilterRepository>(serviceKey, static (serviceProvider, key) =>
			new FileFilterRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		services.AddKeyedScoped<IEntityRepository<FilterModel>>(serviceKey, static (serviceProvider, key) =>
			new FileFilterRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		return services;
	}

	public static IServiceCollection RegisterLiteDbFilterPersistence(this IServiceCollection services)
	{
		services.AddScoped<IFilterRepository, LiteDbFilterRepository>();
		services.AddScoped<IEntityRepository<FilterModel>, LiteDbFilterRepository>();
		return services;
	}

	public static IServiceCollection RegisterLiteDbFilterPersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<IFilterRepository>(serviceKey, static (serviceProvider, key) =>
			new LiteDbFilterRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<FilterModel>>(serviceKey, static (serviceProvider, key) =>
			new LiteDbFilterRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		return services;
	}

	public static IServiceCollection RegisterMongoDbFilterPersistence(this IServiceCollection services)
	{
		services.AddScoped<IFilterRepository, MongoDbFilterRepository>();
		services.AddScoped<IEntityRepository<FilterModel>, MongoDbFilterRepository>();
		return services;
	}

	public static IServiceCollection RegisterMongoDbFilterPersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<IFilterRepository>(serviceKey, static (serviceProvider, key) =>
			new MongoDbFilterRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<FilterModel>>(serviceKey, static (serviceProvider, key) =>
			new MongoDbFilterRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		return services;
	}

	public static IServiceCollection RegisterMySqlFilterPersistence(this IServiceCollection services)
	{
		services.AddScoped<IFilterRepository, MySqlFilterRepository>();
		services.AddScoped<IEntityRepository<FilterModel>, MySqlFilterRepository>();
		return services;
	}

	public static IServiceCollection RegisterMySqlFilterPersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<IFilterRepository>(serviceKey, static (serviceProvider, key) =>
			new MySqlFilterRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<FilterModel>>(serviceKey, static (serviceProvider, key) =>
			new MySqlFilterRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		return services;
	}
}
