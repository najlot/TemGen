using Microsoft.Extensions.DependencyInjection;
using Todo.Service.Infrastructure.Persistence;
using Todo.Service.Infrastructure.Persistence.File;
using Todo.Service.Infrastructure.Persistence.LiteDb;
using Todo.Service.Infrastructure.Persistence.MongoDb;
using Todo.Service.Infrastructure.Persistence.MySql;
using Todo.Service.Shared.Configuration;

namespace Todo.Service.Features.History.Persistence;

public static class HistoryPersistenceServiceCollectionExtensions
{
	public static IServiceCollection RegisterFileHistoryPersistence(this IServiceCollection services)
	{
		services.AddScoped<IHistoryRepository, FileHistoryRepository>();
		services.AddScoped<IEntityRepository<HistoryModel>, FileHistoryRepository>();
		return services;
	}

	public static IServiceCollection RegisterFileHistoryPersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<IHistoryRepository>(serviceKey, static (serviceProvider, key) =>
			new FileHistoryRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		services.AddKeyedScoped<IEntityRepository<HistoryModel>>(serviceKey, static (serviceProvider, key) =>
			new FileHistoryRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		return services;
	}

	public static IServiceCollection RegisterLiteDbHistoryPersistence(this IServiceCollection services)
	{
		services.AddScoped<IHistoryRepository, LiteDbHistoryRepository>();
		services.AddScoped<IEntityRepository<HistoryModel>, LiteDbHistoryRepository>();
		return services;
	}

	public static IServiceCollection RegisterLiteDbHistoryPersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<IHistoryRepository>(serviceKey, static (serviceProvider, key) =>
			new LiteDbHistoryRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<HistoryModel>>(serviceKey, static (serviceProvider, key) =>
			new LiteDbHistoryRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		return services;
	}

	public static IServiceCollection RegisterMongoDbHistoryPersistence(this IServiceCollection services)
	{
		services.AddScoped<IHistoryRepository, MongoDbHistoryRepository>();
		services.AddScoped<IEntityRepository<HistoryModel>, MongoDbHistoryRepository>();
		return services;
	}

	public static IServiceCollection RegisterMongoDbHistoryPersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<IHistoryRepository>(serviceKey, static (serviceProvider, key) =>
			new MongoDbHistoryRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<HistoryModel>>(serviceKey, static (serviceProvider, key) =>
			new MongoDbHistoryRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		return services;
	}

	public static IServiceCollection RegisterMySqlHistoryPersistence(this IServiceCollection services)
	{
		services.AddScoped<IHistoryRepository, MySqlHistoryRepository>();
		services.AddScoped<IEntityRepository<HistoryModel>, MySqlHistoryRepository>();
		return services;
	}

	public static IServiceCollection RegisterMySqlHistoryPersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<IHistoryRepository>(serviceKey, static (serviceProvider, key) =>
			new MySqlHistoryRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<HistoryModel>>(serviceKey, static (serviceProvider, key) =>
			new MySqlHistoryRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		return services;
	}
}
