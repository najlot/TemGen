using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Windows.Controls;
using <#cs Write(Project.Namespace)#>.Client.Data;
using <#cs Write(Project.Namespace)#>.Client.Data.Identity;
using <#cs Write(Project.Namespace)#>.Client.MVVM;
using <#cs Write(Project.Namespace)#>.ClientBase;
using <#cs Write(Project.Namespace)#>.Wpf.Identity;

namespace <#cs Write(Project.Namespace)#>.Wpf;

public static class ServiceProviderFactory
{
	public static ServiceProvider CreateServiceProvider(
		INavigationService navigationService,
		INotificationService notificationService)
	{
		var dataServiceUrl = "http://localhost:5000/";

		var serviceCollection = new ServiceCollection();
		var map = new Najlot.Map.Map()
			.Register<#cs Write(Project.Namespace.Replace(".", ""))#>ClientDataMappings()
			.Register<#cs Write(Project.Namespace.Replace(".", ""))#>ClientBaseMappings();

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
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
