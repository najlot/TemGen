using Microsoft.Extensions.DependencyInjection;
using Todo.Client.Data.Filters;
using Todo.Client.Data.GlobalSearch;
using Todo.Client.Data.History;
using Todo.Client.Data.Identity;
using Todo.Client.Data.Notes;
using Todo.Client.Data.TodoItems;
using Todo.Client.Data.Trash;
using Todo.Client.Data.Users;

namespace Todo.Client.Data;

public static class ServiceCollectionExtensions
{
	extension(IServiceCollection serviceCollection)
	{
		public void RegisterClientData()
		{
			serviceCollection.RegisterClientDataIdentity();
			serviceCollection.RegisterClientDataRepositories();
			serviceCollection.RegisterClientDataServices();
		}

		public void RegisterClientDataIdentity()
		{
			serviceCollection.AddScoped<IPasswordResetService, PasswordResetService>();
			serviceCollection.AddScoped<IRegistrationService, RegistrationService>();
			serviceCollection.AddScoped<ITokenProvider, RefreshingTokenProvider>();
			serviceCollection.AddScoped<ITokenService, TokenService>();
		}

		public void RegisterClientDataRepositories()
		{
			serviceCollection.AddScoped<IFilterRepository, FilterRepository>();
			serviceCollection.AddScoped<IGlobalSearchRepository, GlobalSearchRepository>();
			serviceCollection.AddScoped<IHistoryRepository, HistoryRepository>();
			serviceCollection.AddScoped<ITrashRepository, TrashRepository>();
			serviceCollection.AddScoped<INoteRepository, NoteRepository>();
			serviceCollection.AddScoped<ITodoItemRepository, TodoItemRepository>();
			serviceCollection.AddScoped<IUserRepository, UserRepository>();
		}

		public void RegisterClientDataServices()
		{
			serviceCollection.AddScoped<IApiEventConnectionProvider, ApiEventConnectionProvider>();
			serviceCollection.AddScoped<IFilterService, FilterService>();
			serviceCollection.AddScoped<IGlobalSearchService, GlobalSearchService>();
			serviceCollection.AddScoped<IHistoryService, HistoryService>();
			serviceCollection.AddScoped<ITrashService, TrashService>();
			serviceCollection.AddScoped<INoteService, NoteService>();
			serviceCollection.AddScoped<ITodoItemService, TodoItemService>();
			serviceCollection.AddScoped<IUserService, UserService>();
		}
	}
}