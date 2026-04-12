using Avalonia.Controls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using <# Project.Namespace#>.Client.Data;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.MVVM;
using <# Project.Namespace#>.ClientBase;
using <# Project.Namespace#>.Avalonia.Identity;

namespace <# Project.Namespace#>.Avalonia;

public static class ServiceProviderFactory
{
	public static Func<IUserDataStore>? PlatformUserDataStoreFactory { get; set; }
	public static Func<string?>? PlatformDataServiceUrlFactory { get; set; }

	public static ServiceProvider CreateServiceProvider(
		INavigationService navigationService,
		INotificationService notificationService)
	{
		var assembly = typeof(ServiceProviderFactory).Assembly;
		using var appSettingsStream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.appsettings.json")
			?? throw new InvalidOperationException("Embedded appsettings.json not found.");

		var configurationBuilder = new ConfigurationBuilder()
			.AddJsonStream(appSettingsStream);

		var basePath = AppContext.BaseDirectory;
		var appSettingsPath = Path.Combine(basePath, "appsettings.json");
		if (File.Exists(appSettingsPath))
		{
			configurationBuilder.AddJsonFile(appSettingsPath, optional: true);
		}

		var appSettingsDevelopmentPath = Path.Combine(basePath, "appsettings.development.json");
		if (File.Exists(appSettingsDevelopmentPath))
		{
			configurationBuilder.AddJsonFile(appSettingsDevelopmentPath, optional: true);
		}

		var configuration = configurationBuilder.Build();

		var dataServiceUrl = PlatformDataServiceUrlFactory?.Invoke();
		if (string.IsNullOrWhiteSpace(dataServiceUrl))
		{
			dataServiceUrl = configuration.GetSection("DataServiceUrl")?.Get<string>() ?? throw new InvalidOperationException("DataServiceUrl not found.");
		}

		var serviceCollection = new ServiceCollection();
		var map = new Najlot.Map.Map()
			.Register<#cs Write(Project.Namespace.Replace(".", ""))#>ClientDataMappings()
			.Register<#cs Write(Project.Namespace.Replace(".", ""))#>ClientBaseMappings();

		serviceCollection.AddSingleton(map);
		serviceCollection.AddSingleton<IViewManager<Control>>(sp => new ViewManager(sp));
		serviceCollection.AddSingleton<IUserDataStore>(_ => PlatformUserDataStoreFactory?.Invoke() ?? new PersistentUserDataStore());
		serviceCollection.AddSingleton<IDispatcherHelper, DispatcherHelper>();

		serviceCollection.AddSingleton(navigationService);
		serviceCollection.AddSingleton(notificationService);

		serviceCollection.AddHttpClient(Options.DefaultName, c => c.BaseAddress = new Uri(dataServiceUrl));

		serviceCollection.RegisterClientData();
		serviceCollection.RegisterClientBase();

		var serviceProvider = serviceCollection.BuildServiceProvider();

		map.RegisterFactory(serviceProvider.GetRequiredService);

		return serviceProvider;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>