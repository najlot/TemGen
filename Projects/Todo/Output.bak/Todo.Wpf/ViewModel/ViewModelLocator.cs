using System;
using Microsoft.Extensions.DependencyInjection;
using Todo.Client.Data;
using Todo.Client.MVVM;
using Todo.Client.MVVM.Services;
using Todo.ClientBase;
using Todo.ClientBase.ProfileHandler;
using Todo.ClientBase.Services;
using Todo.ClientBase.Services.Implementation;
using Todo.ClientBase.ViewModel;
using Todo.ClientBase.Models;

namespace Todo.Wpf.ViewModel;

public class ViewModelLocator
{
	public ViewModelLocator()
	{
		var serviceCollection = new ServiceCollection();
		var map = new Najlot.Map.Map().RegisterTodoClientDataMappings().RegisterTodoClientBaseMappings();
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
		serviceCollection.AddTransient((c) => c.GetRequiredService<IProfileHandler>().GetNoteService());
		serviceCollection.AddTransient((c) => c.GetRequiredService<IProfileHandler>().GetTodoItemService());

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
}