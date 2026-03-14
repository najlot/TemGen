using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using <#cs Write(Project.Namespace)#>.Client.Data;
using <#cs Write(Project.Namespace)#>.Client.Data.Identity;
using <#cs Write(Project.Namespace)#>.Client.MVVM;
using <#cs Write(Project.Namespace)#>.ClientBase;
using <#cs Write(Project.Namespace)#>.Maui.Identity;

namespace <#cs Write(Project.Namespace)#>.Maui;

public static class ServiceProviderFactory
{
	public static ServiceProvider CreateServiceProvider(
		INavigationService navigationService,
		INotificationService notificationService)
	{
		var assembly = Assembly.GetExecutingAssembly();
		var configBuilder = new ConfigurationBuilder();

		var appSettingsStream = assembly.GetManifestResourceStream("<#cs Write(Project.Namespace)#>.Maui.appsettings.json");
		if (appSettingsStream is not null)
		{
			configBuilder.AddJsonStream(appSettingsStream);
		}

		var configuration = configBuilder.Build();
		var dataServiceUrl = configuration.GetSection("DataServiceUrl")?.Get<string>() ?? throw new InvalidOperationException("DataServiceUrl not found.");

		var serviceCollection = new ServiceCollection();
		var map = new Najlot.Map.Map()
			.Register<#cs Write(Project.Namespace.Replace(".", ""))#>ClientDataMappings()
			.Register<#cs Write(Project.Namespace.Replace(".", ""))#>ClientBaseMappings();

		serviceCollection.AddSingleton(map);
		serviceCollection.AddSingleton<IViewManager<View>>(sp => new ViewManager(sp));
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
