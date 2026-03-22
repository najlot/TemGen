using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Todo.Service.Configuration;
using Todo.Service.Model;

namespace Todo.Service.Repository.MySqlImpl;

public static class MySqlRepositoryServiceCollectionExtensions
{
	public static IServiceCollection RegisterMySqlRepositories(this IServiceCollection services, MySqlConfiguration configuration)
	{
		return services.RegisterMySqlRepositories(configuration, serviceKey: null);
	}

	public static IServiceCollection RegisterMySqlRepositories(this IServiceCollection services, MySqlConfiguration configuration, object? serviceKey)
	{
		if (serviceKey == null)
		{
			services.AddSingleton(configuration);
			services.AddScoped<MySqlDbContext>();
			services.AddScoped<IUnitOfWork, MySqlUnitOfWork>();
			services.AddScoped<IUserRepository, MySqlUserRepository>();
			services.AddScoped<IEntityRepository<UserModel>, MySqlUserRepository>();
			services.AddScoped<INoteRepository, MySqlNoteRepository>();
			services.AddScoped<IEntityRepository<NoteModel>, MySqlNoteRepository>();
			services.AddScoped<ITodoItemRepository, MySqlTodoItemRepository>();
			services.AddScoped<IEntityRepository<TodoItemModel>, MySqlTodoItemRepository>();

			return services;
		}

		services.AddKeyedSingleton(serviceKey, configuration);
		services.AddKeyedScoped<MySqlDbContext>(serviceKey, static (serviceProvider, key) =>
			new MySqlDbContext(
				serviceProvider.GetRequiredKeyedService<MySqlConfiguration>(key),
				serviceProvider.GetRequiredService<ILoggerFactory>()));
		services.AddKeyedScoped<IUnitOfWork>(serviceKey, static (serviceProvider, key) =>
			new MySqlUnitOfWork(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		services.AddKeyedScoped<IUserRepository>(serviceKey, static (serviceProvider, key) =>
			new MySqlUserRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<UserModel>>(serviceKey, static (serviceProvider, key) =>
			new MySqlUserRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		services.AddKeyedScoped<INoteRepository>(serviceKey, static (serviceProvider, key) =>
			new MySqlNoteRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<NoteModel>>(serviceKey, static (serviceProvider, key) =>
			new MySqlNoteRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		services.AddKeyedScoped<ITodoItemRepository>(serviceKey, static (serviceProvider, key) =>
			new MySqlTodoItemRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<TodoItemModel>>(serviceKey, static (serviceProvider, key) =>
			new MySqlTodoItemRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));


		return services;
	}
}