using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Windows.Controls;
using <# Project.Namespace#>.Client.Data;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.MVVM;
using <# Project.Namespace#>.ClientBase;
using <# Project.Namespace#>.Wpf.Controls;
using <# Project.Namespace#>.Wpf.Identity;
using <# Project.Namespace#>.Wpf.Views;

namespace <# Project.Namespace#>.Wpf;

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
			.Register<#cs Write(Project.Namespace.Replace(".", ""))#>ClientDataMappings()
			.Register<#cs Write(Project.Namespace.Replace(".", ""))#>ClientBaseMappings();

		serviceCollection.AddSingleton(map);
		serviceCollection.AddSingleton<IViewManager<Control>>(sp => new ViewManager(sp));
		serviceCollection.AddSingleton<IUserDataStore, PersistentUserDataStore>();
		serviceCollection.AddSingleton<IDispatcherHelper, DispatcherHelper>();

		serviceCollection.AddSingleton(navigationService);
		serviceCollection.AddSingleton(notificationService);

		serviceCollection.AddHttpClient(Options.DefaultName, c => { c.BaseAddress = new Uri(dataServiceUrl); });

		serviceCollection.RegisterClientData();
		serviceCollection.RegisterClientBase();
		serviceCollection.AddTransient<ToggleFavoriteButton>();
<#for definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>		serviceCollection.AddTransient<<# definition.Name#>View>();
<#end#>

		var serviceProvider = serviceCollection.BuildServiceProvider();

		map.RegisterFactory(serviceProvider.GetRequiredService);

		return serviceProvider;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
