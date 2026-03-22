using Todo.Service.Services.GlobalSearch;
using Todo.Service.Services.Trash;

namespace Todo.Service.Services;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection RegisterServices(this IServiceCollection services)
	{
		services.AddHttpContextAccessor();
		services.AddScoped<IUserIdProvider, HttpContextUserIdProvider>();
		services.AddScoped<IPermissionQueryFilter, PermissionQueryFilter>();
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<TokenService>();

		services.AddScoped<NoteService>();
		services.AddScoped<TodoItemService>();
		
		services.AddScoped<GlobalSearchService>();
		services.AddScoped<IGlobalSearchSource, NoteGlobalSearchSource>();
		services.AddScoped<IGlobalSearchSource, TodoItemGlobalSearchSource>();

		services.AddScoped<TrashService>();
		services.AddScoped<ITrashSource, NoteTrashSource>();
		services.AddScoped<ITrashSource, TodoItemTrashSource>();

		services.AddSignalR();
		services.AddScoped<Publisher>();
		services.AddScoped<IPublisher>(serviceProvider => serviceProvider.GetRequiredService<Publisher>());
		services.AddScoped<IOutboxPublisher>(serviceProvider => serviceProvider.GetRequiredService<Publisher>());
		services.AddHostedService<StorageBackupHostedService>();

		return services;
	}
}