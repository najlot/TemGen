using Microsoft.Extensions.DependencyInjection;
using System;
using Todo.ClientBase.ViewModels;

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
			serviceCollection.AddTransient<GlobalSearchViewModel>();
			serviceCollection.AddTransient<EntityHistoryViewModel>();
			serviceCollection.AddTransient<TrashViewModel>();

			serviceCollection.AddTransient<NotesViewModel>();
			serviceCollection.AddTransient<TodoItemsViewModel>();

			serviceCollection.AddTransient<ChecklistTaskViewModel>();
			serviceCollection.AddTransient<NoteViewModel>();
			serviceCollection.AddTransient<TodoItemViewModel>();
		}
	}
}