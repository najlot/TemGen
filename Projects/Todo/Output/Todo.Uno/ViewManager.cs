using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Client.MVVM;
using Todo.ClientBase;
using Todo.ClientBase.ViewModel;
using Todo.Uno.View;

namespace Todo.Uno;

public class ViewManager(IServiceProvider serviceProvider) : IViewManager<FrameworkElement>
{
	private readonly Dictionary<Type, Type> _knownControls = new()
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

	public FrameworkElement GetView<TViewModel>(TViewModel viewModel) where TViewModel : notnull
	{
		if (!_knownControls.TryGetValue(typeof(TViewModel), out var viewType))
		{
			throw new InvalidOperationException(
				$"No view is registered for view model type '{typeof(TViewModel).FullName}'. " +
				$"Ensure that this view model type is added to the '{nameof(ViewManager)}' {_knownControls.GetType().Name} mapping.");
		}

		if (viewType.GetConstructor(Type.EmptyTypes) is not null)
		{
			if (Activator.CreateInstance(viewType) is FrameworkElement element)
			{
				element.DataContext = viewModel;
				return element;
			}
		}

		if (serviceProvider.GetRequiredService(viewType) is FrameworkElement instance)
		{
			instance.DataContext = viewModel;
			return instance;
		}

		throw new NullReferenceException($"The Class {viewType.FullName} is not a FrameworkElement.");
	}

	public async Task<bool> CanNavigateAsync(FrameworkElement? currentView)
	{
		if (currentView?.DataContext is INavigationGuard navigationGuard)
		{
			return await navigationGuard.CanNavigateAsync().ConfigureAwait(false);
		}

		return true;
	}

	public async Task DisposeView(FrameworkElement? element)
	{
		await DisposeObject(element);
		await DisposeObject(element?.DataContext);
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

