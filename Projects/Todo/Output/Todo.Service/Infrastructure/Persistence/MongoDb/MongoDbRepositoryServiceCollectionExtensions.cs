using Microsoft.Extensions.DependencyInjection;
using Todo.Service.Features.Filters;
using Todo.Service.Features.Filters.Persistence;
using Todo.Service.Features.History;
using Todo.Service.Features.History.Persistence;
using Todo.Service.Features.Users;
using Todo.Service.Features.Users.Persistence;
using Todo.Service.Shared.Configuration;
using Todo.Service.Features.Notes;
using Todo.Service.Features.Notes.Persistence;
using Todo.Service.Features.TodoItems;
using Todo.Service.Features.TodoItems.Persistence;


namespace Todo.Service.Infrastructure.Persistence.MongoDb;

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
			services.RegisterMongoDbFilterPersistence();
			services.RegisterMongoDbHistoryPersistence();
			services.RegisterMongoDbUserPersistence();
			services.RegisterMongoDbNotePersistence();
			services.RegisterMongoDbTodoItemPersistence();

			return services;
		}

		services.AddKeyedSingleton(serviceKey, configuration);
		services.AddKeyedSingleton<MongoDbContext>(serviceKey, static (serviceProvider, key) =>
			new MongoDbContext(serviceProvider.GetRequiredKeyedService<MongoDbConfiguration>(key)));
		services.AddKeyedScoped<IUnitOfWork>(serviceKey, static (_, _) => new MongoDbUnitOfWork());
		services.RegisterMongoDbFilterPersistence(serviceKey);
		services.RegisterMongoDbHistoryPersistence(serviceKey);
		services.RegisterMongoDbUserPersistence(serviceKey);
		services.RegisterMongoDbNotePersistence(serviceKey);
		services.RegisterMongoDbTodoItemPersistence(serviceKey);


		return services;
	}
}