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


namespace Todo.Service.Infrastructure.Persistence.File;

public static class FileRepositoryServiceCollectionExtensions
{
	public static IServiceCollection RegisterFileRepositories(this IServiceCollection services, FileConfiguration configuration)
	{
		return services.RegisterFileRepositories(configuration, serviceKey: null);
	}

	public static IServiceCollection RegisterFileRepositories(this IServiceCollection services, FileConfiguration configuration, object? serviceKey)
	{
		if (serviceKey == null)
		{
			services.AddSingleton(configuration);
			services.AddScoped<IUnitOfWork, FileUnitOfWork>();
			services.RegisterFileHistoryPersistence();
			services.RegisterFileUserPersistence();
			services.RegisterFileNotePersistence();
			services.RegisterFileTodoItemPersistence();

			return services;
		}

		services.AddKeyedSingleton(serviceKey, configuration);
		services.AddKeyedScoped<IUnitOfWork>(serviceKey, static (_, _) => new FileUnitOfWork());
		services.RegisterFileHistoryPersistence(serviceKey);
		services.RegisterFileUserPersistence(serviceKey);
		services.RegisterFileNotePersistence(serviceKey);
		services.RegisterFileTodoItemPersistence(serviceKey);


		return services;
	}
}