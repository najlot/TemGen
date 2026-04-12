using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Todo.Service.Shared.Configuration;
using Todo.Service.Shared.HealthChecks;
using Todo.Service.Serialization;
using Todo.Service.Features.Auth;

namespace Todo.Service;

public static class StartupServiceCollectionExtensions
{
	public static IServiceCollection RegisterApiInfrastructure(this IServiceCollection services, ServiceConfiguration serviceConfig)
	{
		var validationParameters = TokenService.GetValidationParameters(serviceConfig.Secret);

		services.AddAuthentication(x =>
		{
			x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(x =>
		{
			x.RequireHttpsMetadata = false;
			x.TokenValidationParameters = validationParameters;
			x.Events = new JwtBearerEvents
			{
				OnMessageReceived = context =>
				{
					var accessToken = context.Request.Query["access_token"];
					// Blazor WebAssembly doesn't support sending the JWT in the Authorization header when connecting to a SignalR hub,
					// so we need to check the query string for the token when the request is for the hub.
					// If the request is for on of our SignalR hub ...
					if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/events"))
					{
						// Take the token from the query string
						context.Token = accessToken;
					}

					return Task.CompletedTask;
				}
			};
		});

		services.AddAuthorization();
		services.AddControllers()
			.AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.TypeInfoResolverChain.Insert(0, ServiceJsonSerializerContext.Default);
			});
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();
		services.AddHealthChecks()
			.AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
			.AddCheck<StorageReadinessHealthCheck>("storage", tags: ["ready"]);

		return services;
	}
}