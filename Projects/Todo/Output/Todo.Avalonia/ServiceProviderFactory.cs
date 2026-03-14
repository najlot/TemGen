using Avalonia.Controls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Todo.Client.Data;
using Todo.Client.Data.Identity;
using Todo.Client.MVVM;
using Todo.ClientBase;
using Todo.Avalonia.Identity;

namespace Todo.Avalonia;

public static class ServiceProviderFactory
{
	public static ServiceProvider CreateServiceProvider(
		INavigationService navigationService,
		INotificationService notificationService)
	{
		var configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.AddJsonFile("appsettings.development.json", optional: true)
			.Build();

		var dataServiceUrl = configuration.GetSection("DataServiceUrl")?.Get<string>() ?? throw new InvalidOperationException("DataServiceUrl not found.");

		var serviceCollection = new ServiceCollection();
		var map = new Najlot.Map.Map()
			.RegisterTodoClientDataMappings()
			.RegisterTodoClientBaseMappings();

		serviceCollection.AddSingleton(map);
		serviceCollection.AddSingleton<IViewManager<Control>>(sp => new ViewManager(sp));
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

