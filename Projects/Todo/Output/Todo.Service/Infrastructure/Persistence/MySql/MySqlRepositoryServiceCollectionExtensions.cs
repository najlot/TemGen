using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Todo.Service.Features.Filters;
using Todo.Service.Features.Filters.Persistence;
using Todo.Service.Features.History;
using Todo.Service.Features.History.Persistence;
using Todo.Service.Features.Users;
using Todo.Service.Features.Users.Persistence;
using Todo.Service.Infrastructure.Persistence;
using Todo.Service.Shared.Configuration;
using Todo.Service.Features.Notes;
using Todo.Service.Features.Notes.Persistence;
using Todo.Service.Features.TodoItems;
using Todo.Service.Features.TodoItems.Persistence;


namespace Todo.Service.Infrastructure.Persistence.MySql;

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
			services.RegisterMySqlFilterPersistence();
			services.RegisterMySqlHistoryPersistence();
			services.RegisterMySqlUserPersistence();
			services.RegisterMySqlNotePersistence();
			services.RegisterMySqlTodoItemPersistence();

			return services;
		}

		services.AddKeyedSingleton(serviceKey, configuration);
		services.AddKeyedScoped<MySqlDbContext>(serviceKey, static (serviceProvider, key) =>
			new MySqlDbContext(
				serviceProvider.GetRequiredKeyedService<MySqlConfiguration>(key),
				serviceProvider.GetRequiredService<ILoggerFactory>()));
		services.AddKeyedScoped<IUnitOfWork>(serviceKey, static (serviceProvider, key) =>
			new MySqlUnitOfWork(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		services.RegisterMySqlFilterPersistence(serviceKey);
		services.RegisterMySqlHistoryPersistence(serviceKey);
		services.RegisterMySqlUserPersistence(serviceKey);
		services.RegisterMySqlNotePersistence(serviceKey);
		services.RegisterMySqlTodoItemPersistence(serviceKey);


		return services;
	}
}