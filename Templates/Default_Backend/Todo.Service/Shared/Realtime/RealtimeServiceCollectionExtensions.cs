using Microsoft.Extensions.DependencyInjection;

namespace <# Project.Namespace#>.Service.Shared.Realtime;

public static class RealtimeServiceCollectionExtensions
{
	public static IServiceCollection RegisterRealtimeServices(this IServiceCollection services)
	{
		services.AddSignalR();
		services.AddScoped<Publisher>();
		services.AddScoped<IPublisher>(serviceProvider => serviceProvider.GetRequiredService<Publisher>());
		services.AddScoped<IOutboxPublisher>(serviceProvider => serviceProvider.GetRequiredService<Publisher>());

		return services;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>