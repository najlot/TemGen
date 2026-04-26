using Microsoft.Extensions.DependencyInjection;

namespace Todo.Service.Features.Auth;

public static class AuthServiceCollectionExtensions
{
	public static IServiceCollection RegisterAuthFeature(this IServiceCollection services)
	{
		services.AddHttpContextAccessor();
		services.AddScoped<IUserIdProvider, HttpContextUserIdProvider>();
		services.AddScoped<IPasswordResetCodeSender, SmtpPasswordResetCodeSender>();
		services.AddScoped<IPermissionQueryFilter, PermissionQueryFilter>();
		services.AddScoped<PasswordResetService>();
		services.AddScoped<TokenService>();

		return services;
	}
}
