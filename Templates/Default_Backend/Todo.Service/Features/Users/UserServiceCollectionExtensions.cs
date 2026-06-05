using Microsoft.Extensions.DependencyInjection;

namespace <# Project.Namespace#>.Service.Features.Users;

public static class UserServiceCollectionExtensions
{
	public static IServiceCollection RegisterUsersFeature(this IServiceCollection services)
	{
		services.AddScoped<IUserService, UserService>();
		return services;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>