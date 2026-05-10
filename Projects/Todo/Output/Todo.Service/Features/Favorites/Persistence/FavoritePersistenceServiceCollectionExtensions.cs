using Microsoft.Extensions.DependencyInjection;
using Todo.Service.Infrastructure.Persistence;
using Todo.Service.Infrastructure.Persistence.File;
using Todo.Service.Infrastructure.Persistence.LiteDb;
using Todo.Service.Infrastructure.Persistence.MongoDb;
using Todo.Service.Infrastructure.Persistence.MySql;
using Todo.Service.Shared.Configuration;

namespace Todo.Service.Features.Favorites.Persistence;

public static class FavoritePersistenceServiceCollectionExtensions
{
	public static IServiceCollection RegisterFileFavoritePersistence(this IServiceCollection services)
	{
		services.AddScoped<IFavoriteRepository, FileFavoriteRepository>();
		services.AddScoped<IEntityRepository<FavoriteModel>, FileFavoriteRepository>();
		return services;
	}

	public static IServiceCollection RegisterFileFavoritePersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<IFavoriteRepository>(serviceKey, static (serviceProvider, key) =>
			new FileFavoriteRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		services.AddKeyedScoped<IEntityRepository<FavoriteModel>>(serviceKey, static (serviceProvider, key) =>
			new FileFavoriteRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		return services;
	}

	public static IServiceCollection RegisterLiteDbFavoritePersistence(this IServiceCollection services)
	{
		services.AddScoped<IFavoriteRepository, LiteDbFavoriteRepository>();
		services.AddScoped<IEntityRepository<FavoriteModel>, LiteDbFavoriteRepository>();
		return services;
	}

	public static IServiceCollection RegisterLiteDbFavoritePersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<IFavoriteRepository>(serviceKey, static (serviceProvider, key) =>
			new LiteDbFavoriteRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<FavoriteModel>>(serviceKey, static (serviceProvider, key) =>
			new LiteDbFavoriteRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		return services;
	}

	public static IServiceCollection RegisterMongoDbFavoritePersistence(this IServiceCollection services)
	{
		services.AddScoped<IFavoriteRepository, MongoDbFavoriteRepository>();
		services.AddScoped<IEntityRepository<FavoriteModel>, MongoDbFavoriteRepository>();
		return services;
	}

	public static IServiceCollection RegisterMongoDbFavoritePersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<IFavoriteRepository>(serviceKey, static (serviceProvider, key) =>
			new MongoDbFavoriteRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<FavoriteModel>>(serviceKey, static (serviceProvider, key) =>
			new MongoDbFavoriteRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		return services;
	}

	public static IServiceCollection RegisterMySqlFavoritePersistence(this IServiceCollection services)
	{
		services.AddScoped<IFavoriteRepository, MySqlFavoriteRepository>();
		services.AddScoped<IEntityRepository<FavoriteModel>, MySqlFavoriteRepository>();
		return services;
	}

	public static IServiceCollection RegisterMySqlFavoritePersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<IFavoriteRepository>(serviceKey, static (serviceProvider, key) =>
			new MySqlFavoriteRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<FavoriteModel>>(serviceKey, static (serviceProvider, key) =>
			new MySqlFavoriteRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		return services;
	}
}
