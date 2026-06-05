using Microsoft.Extensions.DependencyInjection;
using <# Project.Namespace#>.Service.Infrastructure.Persistence;
using <# Project.Namespace#>.Service.Infrastructure.Persistence.File;
using <# Project.Namespace#>.Service.Infrastructure.Persistence.LiteDb;
using <# Project.Namespace#>.Service.Infrastructure.Persistence.MongoDb;
using <# Project.Namespace#>.Service.Infrastructure.Persistence.MySql;
using <# Project.Namespace#>.Service.Shared.Configuration;

namespace <# Project.Namespace#>.Service.Features.Users.Persistence;

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
<#cs SetOutputPathAndSkipOtherDefinitions()#>