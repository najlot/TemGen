using Microsoft.Extensions.DependencyInjection;
using Todo.Service.Configuration;
using Todo.Service.Model;

namespace Todo.Service.Repository.LiteDbImpl;

public static class LiteDbRepositoryServiceCollectionExtensions
{
	public static IServiceCollection RegisterLiteDbRepositories(this IServiceCollection services, LiteDbConfiguration configuration)
	{
		return services.RegisterLiteDbRepositories(configuration, serviceKey: null);
	}

	public static IServiceCollection RegisterLiteDbRepositories(this IServiceCollection services, LiteDbConfiguration configuration, object? serviceKey)
	{
		if (serviceKey == null)
		{
			services.AddSingleton(configuration);
			services.AddSingleton<LiteDbContext>();
			services.AddScoped<IUnitOfWork, LiteDbUnitOfWork>();
			services.AddScoped<IUserRepository, LiteDbUserRepository>();
			services.AddScoped<IEntityRepository<UserModel>, LiteDbUserRepository>();
			services.AddScoped<INoteRepository, LiteDbNoteRepository>();
			services.AddScoped<IEntityRepository<NoteModel>, LiteDbNoteRepository>();
			services.AddScoped<ITodoItemRepository, LiteDbTodoItemRepository>();
			services.AddScoped<IEntityRepository<TodoItemModel>, LiteDbTodoItemRepository>();

			return services;
		}

		services.AddKeyedSingleton(serviceKey, configuration);
		services.AddKeyedSingleton<LiteDbContext>(serviceKey, static (serviceProvider, key) =>
			new LiteDbContext(serviceProvider.GetRequiredKeyedService<LiteDbConfiguration>(key)));
		services.AddKeyedScoped<IUnitOfWork>(serviceKey, static (serviceProvider, key) =>
			new LiteDbUnitOfWork(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.AddKeyedScoped<IUserRepository>(serviceKey, static (serviceProvider, key) =>
			new LiteDbUserRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<UserModel>>(serviceKey, static (serviceProvider, key) =>
			new LiteDbUserRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.AddKeyedScoped<INoteRepository>(serviceKey, static (serviceProvider, key) =>
			new LiteDbNoteRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<NoteModel>>(serviceKey, static (serviceProvider, key) =>
			new LiteDbNoteRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.AddKeyedScoped<ITodoItemRepository>(serviceKey, static (serviceProvider, key) =>
			new LiteDbTodoItemRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<TodoItemModel>>(serviceKey, static (serviceProvider, key) =>
			new LiteDbTodoItemRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));


		return services;
	}
}