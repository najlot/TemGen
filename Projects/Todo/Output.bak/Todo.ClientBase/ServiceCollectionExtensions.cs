using Microsoft.Extensions.DependencyInjection;
using Todo.ClientBase.ViewModel;
using System;

namespace Todo.ClientBase;

public static class ServiceCollectionExtensions
{
	public static void RegisterViewModels(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddSingleton<LoginViewModel>();
		serviceCollection.AddTransient<ProfileViewModel>();
		serviceCollection.AddSingleton<Func<ProfileViewModel>>(c => () => c.GetRequiredService<ProfileViewModel>());
		serviceCollection.AddTransient<LoginProfileViewModel>();
		serviceCollection.AddSingleton<Func<LoginProfileViewModel>>(c => () => c.GetRequiredService<LoginProfileViewModel>());
		serviceCollection.AddScoped<MenuViewModel>();

		serviceCollection.AddScoped<AllNotesViewModel>();
		serviceCollection.AddScoped<AllTodoItemsViewModel>();

		serviceCollection.AddTransient<NoteViewModel>();
		serviceCollection.AddSingleton<Func<NoteViewModel>>(c => () => c.GetRequiredService<NoteViewModel>());
		serviceCollection.AddTransient<ChecklistTaskViewModel>();
		serviceCollection.AddSingleton<Func<ChecklistTaskViewModel>>(c => () => c.GetRequiredService<ChecklistTaskViewModel>());
		serviceCollection.AddTransient<TodoItemViewModel>();
		serviceCollection.AddSingleton<Func<TodoItemViewModel>>(c => () => c.GetRequiredService<TodoItemViewModel>());
	}
}