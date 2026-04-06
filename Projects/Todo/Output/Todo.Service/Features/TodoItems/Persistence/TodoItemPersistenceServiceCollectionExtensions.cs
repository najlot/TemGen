using Microsoft.Extensions.DependencyInjection;
using Todo.Service.Infrastructure.Persistence;
using Todo.Service.Infrastructure.Persistence.File;
using Todo.Service.Infrastructure.Persistence.LiteDb;
using Todo.Service.Infrastructure.Persistence.MongoDb;
using Todo.Service.Infrastructure.Persistence.MySql;
using Todo.Service.Shared.Configuration;

namespace Todo.Service.Features.TodoItems.Persistence;

public static class TodoItemPersistenceServiceCollectionExtensions
{
	public static IServiceCollection RegisterFileTodoItemPersistence(this IServiceCollection services)
	{
		services.AddScoped<ITodoItemRepository, FileTodoItemRepository>();
		services.AddScoped<IEntityRepository<TodoItemModel>, FileTodoItemRepository>();
		return services;
	}

	public static IServiceCollection RegisterFileTodoItemPersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<ITodoItemRepository>(serviceKey, static (serviceProvider, key) =>
			new FileTodoItemRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		services.AddKeyedScoped<IEntityRepository<TodoItemModel>>(serviceKey, static (serviceProvider, key) =>
			new FileTodoItemRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		return services;
	}

	public static IServiceCollection RegisterLiteDbTodoItemPersistence(this IServiceCollection services)
	{
		services.AddScoped<ITodoItemRepository, LiteDbTodoItemRepository>();
		services.AddScoped<IEntityRepository<TodoItemModel>, LiteDbTodoItemRepository>();
		return services;
	}

	public static IServiceCollection RegisterLiteDbTodoItemPersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<ITodoItemRepository>(serviceKey, static (serviceProvider, key) =>
			new LiteDbTodoItemRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<TodoItemModel>>(serviceKey, static (serviceProvider, key) =>
			new LiteDbTodoItemRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		return services;
	}

	public static IServiceCollection RegisterMongoDbTodoItemPersistence(this IServiceCollection services)
	{
		services.AddScoped<ITodoItemRepository, MongoDbTodoItemRepository>();
		services.AddScoped<IEntityRepository<TodoItemModel>, MongoDbTodoItemRepository>();
		return services;
	}

	public static IServiceCollection RegisterMongoDbTodoItemPersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<ITodoItemRepository>(serviceKey, static (serviceProvider, key) =>
			new MongoDbTodoItemRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<TodoItemModel>>(serviceKey, static (serviceProvider, key) =>
			new MongoDbTodoItemRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		return services;
	}

	public static IServiceCollection RegisterMySqlTodoItemPersistence(this IServiceCollection services)
	{
		services.AddScoped<ITodoItemRepository, MySqlTodoItemRepository>();
		services.AddScoped<IEntityRepository<TodoItemModel>, MySqlTodoItemRepository>();
		return services;
	}

	public static IServiceCollection RegisterMySqlTodoItemPersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<ITodoItemRepository>(serviceKey, static (serviceProvider, key) =>
			new MySqlTodoItemRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<TodoItemModel>>(serviceKey, static (serviceProvider, key) =>
			new MySqlTodoItemRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		return services;
	}
}
