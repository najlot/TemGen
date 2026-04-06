using Microsoft.Extensions.DependencyInjection;

namespace Todo.Service.Features.Users;

public static class UserServiceCollectionExtensions
{
	public static IServiceCollection RegisterUsersFeature(this IServiceCollection services)
	{
		services.AddScoped<IUserService, UserService>();
		return services;
	}
}
