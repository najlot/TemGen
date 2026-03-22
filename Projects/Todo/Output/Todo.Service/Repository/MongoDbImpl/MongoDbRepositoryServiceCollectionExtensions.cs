using Microsoft.Extensions.DependencyInjection;
using Todo.Service.Configuration;
using Todo.Service.Model;

namespace Todo.Service.Repository.MongoDbImpl;

public static class MongoDbRepositoryServiceCollectionExtensions
{
	public static IServiceCollection RegisterMongoDbRepositories(this IServiceCollection services, MongoDbConfiguration configuration)
	{
		return services.RegisterMongoDbRepositories(configuration, serviceKey: null);
	}

	public static IServiceCollection RegisterMongoDbRepositories(this IServiceCollection services, MongoDbConfiguration configuration, object? serviceKey)
	{
		if (serviceKey == null)
		{
			services.AddSingleton(configuration);
			services.AddSingleton<MongoDbContext>();
			services.AddScoped<IUnitOfWork, MongoDbUnitOfWork>();
			services.AddScoped<IUserRepository, MongoDbUserRepository>();
			services.AddScoped<IEntityRepository<UserModel>, MongoDbUserRepository>();
			services.AddScoped<INoteRepository, MongoDbNoteRepository>();
			services.AddScoped<IEntityRepository<NoteModel>, MongoDbNoteRepository>();
			services.AddScoped<ITodoItemRepository, MongoDbTodoItemRepository>();
			services.AddScoped<IEntityRepository<TodoItemModel>, MongoDbTodoItemRepository>();

			return services;
		}

		services.AddKeyedSingleton(serviceKey, configuration);
		services.AddKeyedSingleton<MongoDbContext>(serviceKey, static (serviceProvider, key) =>
			new MongoDbContext(serviceProvider.GetRequiredKeyedService<MongoDbConfiguration>(key)));
		services.AddKeyedScoped<IUnitOfWork>(serviceKey, static (_, _) => new MongoDbUnitOfWork());
		services.AddKeyedScoped<IUserRepository>(serviceKey, static (serviceProvider, key) =>
			new MongoDbUserRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<UserModel>>(serviceKey, static (serviceProvider, key) =>
			new MongoDbUserRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		services.AddKeyedScoped<INoteRepository>(serviceKey, static (serviceProvider, key) =>
			new MongoDbNoteRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<NoteModel>>(serviceKey, static (serviceProvider, key) =>
			new MongoDbNoteRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		services.AddKeyedScoped<ITodoItemRepository>(serviceKey, static (serviceProvider, key) =>
			new MongoDbTodoItemRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<TodoItemModel>>(serviceKey, static (serviceProvider, key) =>
			new MongoDbTodoItemRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));


		return services;
	}
}