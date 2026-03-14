using Microsoft.Extensions.DependencyInjection;
using System;
using Todo.ClientBase.ViewModel;

namespace Todo.ClientBase;

public static class ServiceCollectionExtensions
{
	extension(IServiceCollection serviceCollection)
	{
		public void RegisterClientBase()
		{
			serviceCollection.RegisterClientBaseViewModels();
		}

		public void RegisterClientBaseViewModels()
		{
			serviceCollection.AddTransient(typeof(ViewModelBaseParameters<>));

			serviceCollection.AddTransient<LoginViewModel>();
			serviceCollection.AddTransient<RegisterViewModel>();
			serviceCollection.AddTransient<MenuViewModel>();
			serviceCollection.AddTransient<ManageViewModel>();

			serviceCollection.AddTransient<AllTodoItemsViewModel>();
			serviceCollection.AddTransient<AllNotesViewModel>();

			serviceCollection.AddTransient<TodoItemViewModel>();
			serviceCollection.AddTransient<ChecklistTaskViewModel>();
			serviceCollection.AddTransient<NoteViewModel>();
		}
	}
}