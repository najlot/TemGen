using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Reflection;
using Todo.Client.Data;
using Todo.Client.Data.Identity;
using Todo.Client.MVVM;
using Todo.ClientBase;
using Todo.Uno.Identity;

namespace Todo.Uno;

public static class ServiceProviderFactory
{
	public static ServiceProvider CreateServiceProvider(
		INavigationService navigationService,
		INotificationService notificationService)
	{
		var assembly = Assembly.GetExecutingAssembly();
		var configBuilder = new ConfigurationBuilder();

		using var appSettingsStream = assembly.GetManifestResourceStream("Todo.Uno.appsettings.json");
		if (appSettingsStream is not null)
		{
			configBuilder.AddJsonStream(appSettingsStream);
		}

		using var devSettingsStream = assembly.GetManifestResourceStream("Todo.Uno.appsettings.development.json");
		if (devSettingsStream is not null)
		{
			configBuilder.AddJsonStream(devSettingsStream);
		}

		var configuration = configBuilder.Build();

		var dataServiceUrl = configuration["DataServiceUrl"] ?? throw new InvalidOperationException("DataServiceUrl not found.");

		var serviceCollection = new ServiceCollection();
		var map = new Najlot.Map.Map()
			.RegisterTodoClientDataMappings()
			.RegisterTodoClientBaseMappings();

		serviceCollection.AddSingleton(map);
		serviceCollection.AddSingleton<IViewManager<FrameworkElement>>(sp => new ViewManager(sp));
		serviceCollection.AddSingleton<IUserDataStore, MemoryUserDataStore>();
		serviceCollection.AddSingleton<IDispatcherHelper, DispatcherHelper>();

		serviceCollection.AddSingleton(navigationService);
		serviceCollection.AddSingleton(notificationService);

		serviceCollection.AddHttpClient(Options.DefaultName, c => { c.BaseAddress = new Uri(dataServiceUrl); });

		serviceCollection.RegisterClientData();
		serviceCollection.RegisterClientBase();

		var serviceProvider = serviceCollection.BuildServiceProvider();

		map.RegisterFactory(serviceProvider.GetRequiredService);

		return serviceProvider;
	}
}

