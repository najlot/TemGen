using System;
using Microsoft.Extensions.DependencyInjection;
using <#cs Write(Project.Namespace)#>.Client.Data;
using <#cs Write(Project.Namespace)#>.Client.MVVM;
using <#cs Write(Project.Namespace)#>.Client.MVVM.Services;
using <#cs Write(Project.Namespace)#>.ClientBase;
using <#cs Write(Project.Namespace)#>.ClientBase.ProfileHandler;
using <#cs Write(Project.Namespace)#>.ClientBase.Services;
using <#cs Write(Project.Namespace)#>.ClientBase.Services.Implementation;
using <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;
using <#cs Write(Project.Namespace)#>.ClientBase.Models;

namespace <#cs Write(Project.Namespace)#>.Wpf.ViewModel;

public class ViewModelLocator
{
	public ViewModelLocator()
	{
		var serviceCollection = new ServiceCollection();
		var map = new Najlot.Map.Map().RegisterDataMappings().RegisterViewModelMappings();
		serviceCollection.AddSingleton(map);

		// Register services
		serviceCollection.AddSingleton<IDispatcherHelper, DispatcherHelper>();
		serviceCollection.AddSingleton<INavigationService>(Main);
		serviceCollection.AddSingleton<IErrorService, ErrorService>();
		serviceCollection.AddSingleton<IProfilesService, ProfilesService>();
		serviceCollection.AddSingleton<IMessenger, Messenger>();

		serviceCollection.AddSingleton(c => c.GetRequiredKeyedService<IProfileHandler>(nameof(Source.Local)));
		serviceCollection.AddKeyedSingleton<IProfileHandler, LocalProfileHandler>(nameof(Source.Local));
		serviceCollection.AddKeyedSingleton<IProfileHandler, RestProfileHandler>(nameof(Source.REST));
		serviceCollection.AddKeyedSingleton<IProfileHandler, RmqProfileHandler>(nameof(Source.RMQ));

		serviceCollection.AddTransient((c) => c.GetRequiredService<IProfileHandler>().GetUserService());
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsArray
|| d.IsEnumeration
|| d.IsOwnedType
|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"		serviceCollection.AddTransient((c) => c.GetRequiredService<IProfileHandler>().Get{definition.Name}Service());");
}
#>
		// Register views and view models
		serviceCollection.RegisterViewModels();

		var serviceProvider = serviceCollection.BuildServiceProvider();

		serviceProvider.GetRequiredService<Najlot.Map.IMap>().RegisterFactory(t =>
		{
			if (t.GetConstructor(Type.EmptyTypes) is not null)
			{
				return Activator.CreateInstance(t) ?? throw new NullReferenceException("Could not create " + t.FullName);
			}

			return serviceProvider.GetRequiredService(t);
		});

		var localProfileHandler = serviceProvider.GetRequiredKeyedService<IProfileHandler>(nameof(Source.Local));
		var restProfileHandler = serviceProvider.GetRequiredKeyedService<IProfileHandler>(nameof(Source.REST));
		var rmqProfileHandler = serviceProvider.GetRequiredKeyedService<IProfileHandler>(nameof(Source.RMQ));
		localProfileHandler.SetNext(restProfileHandler).SetNext(rmqProfileHandler);

		var loginViewModel = serviceProvider.GetRequiredService<LoginViewModel>();
		serviceProvider.GetRequiredService<INavigationService>().NavigateForward(loginViewModel);
	}

	public MainViewModel Main { get; } = new MainViewModel();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>