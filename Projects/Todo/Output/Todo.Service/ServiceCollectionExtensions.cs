using Todo.Service.Features.Auth;
using Todo.Service.Features.Filters;
using Todo.Service.Features.GlobalSearch;
using Todo.Service.Features.History;
using Todo.Service.Features.Trash;
using Todo.Service.Features.Users;
using Todo.Service.Infrastructure.StorageBackup;
using Todo.Service.Shared.Realtime;
using Todo.Service.Features.Notes;
using Todo.Service.Features.TodoItems;


namespace Todo.Service;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection RegisterServices(this IServiceCollection services)
	{
		services.RegisterAuthFeature();
		services.RegisterUsersFeature();
		services.RegisterFiltersFeature();
		services.RegisterHistoryFeature();

		services.RegisterNoteFeature();
		services.RegisterTodoItemFeature();

		services.RegisterGlobalSearchFeature();
		services.RegisterTrashFeature();
		services.RegisterRealtimeServices();
		services.RegisterStorageBackupInfrastructure();

		return services;
	}
}