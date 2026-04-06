using Microsoft.Extensions.DependencyInjection;
using Todo.Service.Infrastructure.Persistence;
using Todo.Service.Infrastructure.Persistence.File;
using Todo.Service.Infrastructure.Persistence.LiteDb;
using Todo.Service.Infrastructure.Persistence.MongoDb;
using Todo.Service.Infrastructure.Persistence.MySql;
using Todo.Service.Shared.Configuration;

namespace Todo.Service.Features.Users.Persistence;

public static class UserPersistenceServiceCollectionExtensions
{
	public static IServiceCollection RegisterFileUserPersistence(this IServiceCollection services)
	{
		services.AddScoped<IUserRepository, FileUserRepository>();
		services.AddScoped<IEntityRepository<UserModel>, FileUserRepository>();
		return services;
	}

	public static IServiceCollection RegisterFileUserPersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<IUserRepository>(serviceKey, static (serviceProvider, key) =>
			new FileUserRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		services.AddKeyedScoped<IEntityRepository<UserModel>>(serviceKey, static (serviceProvider, key) =>
			new FileUserRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		return services;
	}

	public static IServiceCollection RegisterLiteDbUserPersistence(this IServiceCollection services)
	{
		services.AddScoped<IUserRepository, LiteDbUserRepository>();
		services.AddScoped<IEntityRepository<UserModel>, LiteDbUserRepository>();
		return services;
	}

	public static IServiceCollection RegisterLiteDbUserPersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<IUserRepository>(serviceKey, static (serviceProvider, key) =>
			new LiteDbUserRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<UserModel>>(serviceKey, static (serviceProvider, key) =>
			new LiteDbUserRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		return services;
	}

	public static IServiceCollection RegisterMongoDbUserPersistence(this IServiceCollection services)
	{
		services.AddScoped<IUserRepository, MongoDbUserRepository>();
		services.AddScoped<IEntityRepository<UserModel>, MongoDbUserRepository>();
		return services;
	}

	public static IServiceCollection RegisterMongoDbUserPersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<IUserRepository>(serviceKey, static (serviceProvider, key) =>
			new MongoDbUserRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<UserModel>>(serviceKey, static (serviceProvider, key) =>
			new MongoDbUserRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		return services;
	}

	public static IServiceCollection RegisterMySqlUserPersistence(this IServiceCollection services)
	{
		services.AddScoped<IUserRepository, MySqlUserRepository>();
		services.AddScoped<IEntityRepository<UserModel>, MySqlUserRepository>();
		return services;
	}

	public static IServiceCollection RegisterMySqlUserPersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<IUserRepository>(serviceKey, static (serviceProvider, key) =>
			new MySqlUserRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<UserModel>>(serviceKey, static (serviceProvider, key) =>
			new MySqlUserRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		return services;
	}
}
