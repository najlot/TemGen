using Microsoft.Extensions.DependencyInjection;
using Todo.Service.Configuration;
using Todo.Service.Model;

namespace Todo.Service.Repository.FileImpl;

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
			services.AddScoped<IUserRepository, FileUserRepository>();
			services.AddScoped<IEntityRepository<UserModel>, FileUserRepository>();
			services.AddScoped<INoteRepository, FileNoteRepository>();
			services.AddScoped<IEntityRepository<NoteModel>, FileNoteRepository>();
			services.AddScoped<ITodoItemRepository, FileTodoItemRepository>();
			services.AddScoped<IEntityRepository<TodoItemModel>, FileTodoItemRepository>();

			return services;
		}

		services.AddKeyedSingleton(serviceKey, configuration);
		services.AddKeyedScoped<IUnitOfWork>(serviceKey, static (_, _) => new FileUnitOfWork());
		services.AddKeyedScoped<IUserRepository>(serviceKey, static (serviceProvider, key) =>
			new FileUserRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		services.AddKeyedScoped<IEntityRepository<UserModel>>(serviceKey, static (serviceProvider, key) =>
			new FileUserRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		services.AddKeyedScoped<INoteRepository>(serviceKey, static (serviceProvider, key) =>
			new FileNoteRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		services.AddKeyedScoped<IEntityRepository<NoteModel>>(serviceKey, static (serviceProvider, key) =>
			new FileNoteRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		services.AddKeyedScoped<ITodoItemRepository>(serviceKey, static (serviceProvider, key) =>
			new FileTodoItemRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		services.AddKeyedScoped<IEntityRepository<TodoItemModel>>(serviceKey, static (serviceProvider, key) =>
			new FileTodoItemRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));


		return services;
	}
}