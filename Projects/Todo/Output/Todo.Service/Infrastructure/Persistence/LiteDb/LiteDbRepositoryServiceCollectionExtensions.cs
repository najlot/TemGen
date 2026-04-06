using Microsoft.Extensions.DependencyInjection;
using Todo.Service.Features.History;
using Todo.Service.Features.History.Persistence;
using Todo.Service.Features.Users;
using Todo.Service.Features.Users.Persistence;
using Todo.Service.Shared.Configuration;
using Todo.Service.Features.Notes;
using Todo.Service.Features.Notes.Persistence;
using Todo.Service.Features.TodoItems;
using Todo.Service.Features.TodoItems.Persistence;


namespace Todo.Service.Infrastructure.Persistence.LiteDb;

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
			services.RegisterLiteDbHistoryPersistence();
			services.RegisterLiteDbUserPersistence();
			services.RegisterLiteDbNotePersistence();
			services.RegisterLiteDbTodoItemPersistence();

			return services;
		}

		services.AddKeyedSingleton(serviceKey, configuration);
		services.AddKeyedSingleton<LiteDbContext>(serviceKey, static (serviceProvider, key) =>
			new LiteDbContext(serviceProvider.GetRequiredKeyedService<LiteDbConfiguration>(key)));
		services.AddKeyedScoped<IUnitOfWork>(serviceKey, static (serviceProvider, key) =>
			new LiteDbUnitOfWork(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.RegisterLiteDbHistoryPersistence(serviceKey);
		services.RegisterLiteDbUserPersistence(serviceKey);
		services.RegisterLiteDbNotePersistence(serviceKey);
		services.RegisterLiteDbTodoItemPersistence(serviceKey);


		return services;
	}
}