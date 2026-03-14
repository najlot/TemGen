using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Client.MVVM;
using Todo.ClientBase;
using Todo.ClientBase.ViewModel;
using Todo.Maui.View;

namespace Todo.Maui;

public class ViewManager(IServiceProvider serviceProvider) : IViewManager<View>
{
	private readonly Dictionary<Type, Type> _knownViews = new()
	{
		[typeof(LoginViewModel)] = typeof(LoginView),
		[typeof(RegisterViewModel)] = typeof(RegisterView),
		[typeof(MenuViewModel)] = typeof(MenuView),
		[typeof(ManageViewModel)] = typeof(ManageView),
		[typeof(AllTodoItemsViewModel)] = typeof(AllTodoItemsView),
		[typeof(AllNotesViewModel)] = typeof(AllNotesView),
		[typeof(TodoItemViewModel)] = typeof(TodoItemView),
		[typeof(ChecklistTaskViewModel)] = typeof(ChecklistTaskView),
		[typeof(NoteViewModel)] = typeof(NoteView),
	};

	public View GetView<TViewModel>(TViewModel viewModel) where TViewModel : notnull
	{
		if (!_knownViews.TryGetValue(typeof(TViewModel), out var viewType))
		{
			throw new InvalidOperationException(
				$"No view is registered for view model type '{typeof(TViewModel).FullName}'. " +
				$"Ensure that this view model type is added to the '{nameof(ViewManager)}' {_knownViews.GetType().Name} mapping.");
		}

		if (viewType.GetConstructor(Type.EmptyTypes) is not null)
		{
			if (Activator.CreateInstance(viewType) is View view)
			{
				view.BindingContext = viewModel;
				return view;
			}
		}

		if (serviceProvider.GetRequiredService(viewType) is View instance)
		{
			instance.BindingContext = viewModel;
			return instance;
		}

		throw new NullReferenceException($"The Class {viewType.FullName} is not a View.");
	}

	public async Task<bool> CanNavigateAsync(View? currentView)
	{
		if (currentView?.BindingContext is INavigationGuard navigationGuard)
		{
			return await navigationGuard.CanNavigateAsync().ConfigureAwait(false);
		}

		return true;
	}

	public async Task DisposeView(View? view)
	{
		await DisposeObject(view);
		await DisposeObject(view?.BindingContext);
	}

	private static async Task DisposeObject(object? obj)
	{
		if (obj is IAsyncDisposable asyncDisposable)
		{
			await asyncDisposable.DisposeAsync();
		}
		else if (obj is IDisposable disposable)
		{
			disposable.Dispose();
		}
	}
}
