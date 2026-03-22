using Microsoft.Extensions.DependencyInjection;
using Todo.Client.Data.Identity;
using Todo.Client.Data.Repositories;
using Todo.Client.Data.Repositories.Implementation;
using Todo.Client.Data.Services;
using Todo.Client.Data.Services.Implementation;

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
			serviceCollection.AddScoped<IRegistrationService, RegistrationService>();
			serviceCollection.AddScoped<ITokenProvider, RefreshingTokenProvider>();
			serviceCollection.AddScoped<ITokenService, TokenService>();
		}

		public void RegisterClientDataRepositories()
		{
			serviceCollection.AddScoped<IGlobalSearchRepository, GlobalSearchRepository>();
			serviceCollection.AddScoped<ITrashRepository, TrashRepository>();
			serviceCollection.AddScoped<INoteRepository, NoteRepository>();
			serviceCollection.AddScoped<ITodoItemRepository, TodoItemRepository>();
			serviceCollection.AddScoped<IUserRepository, UserRepository>();
		}

		public void RegisterClientDataServices()
		{
			serviceCollection.AddScoped<IApiEventConnectionProvider, ApiEventConnectionProvider>();
			serviceCollection.AddScoped<IGlobalSearchService, GlobalSearchService>();
			serviceCollection.AddScoped<ITrashService, TrashService>();
			serviceCollection.AddScoped<INoteService, NoteService>();
			serviceCollection.AddScoped<ITodoItemService, TodoItemService>();
			serviceCollection.AddScoped<IUserService, UserService>();
		}
	}
}