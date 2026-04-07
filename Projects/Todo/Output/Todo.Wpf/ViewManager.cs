using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using Todo.Client.MVVM;
using Todo.ClientBase;
using Todo.ClientBase.ViewModels;
using Todo.Wpf.Views;

namespace Todo.Wpf;

public class ViewManager(IServiceProvider serviceProvider) : IViewManager<Control>
{
	private readonly Dictionary<Type, Type> _knownControls = new()
	{
		[typeof(LoginViewModel)] = typeof(LoginView),
		[typeof(RegisterViewModel)] = typeof(RegisterView),
		[typeof(MenuViewModel)] = typeof(MenuView),
		[typeof(ManageViewModel)] = typeof(ManageView),
		[typeof(GlobalSearchViewModel)] = typeof(GlobalSearchView),
		[typeof(EntityHistoryViewModel)] = typeof(HistoryView),
		[typeof(TrashViewModel)] = typeof(TrashView),
		[typeof(NotesViewModel)] = typeof(NotesView),
		[typeof(TodoItemsViewModel)] = typeof(TodoItemsView),
		[typeof(NoteViewModel)] = typeof(NoteView),
		[typeof(TodoItemViewModel)] = typeof(TodoItemView),
	};

	public Control GetView<TViewModel>(TViewModel viewModel) where TViewModel : notnull
	{
		if (!_knownControls.TryGetValue(typeof(TViewModel), out var viewType))
		{
			throw new InvalidOperationException(
				$"No view is registered for view model type '{typeof(TViewModel).FullName}'. " +
				$"Ensure that this view model type is added to the '{nameof(ViewManager)}' {_knownControls.GetType().Name} mapping.");
		}

		if (viewType.GetConstructor(Type.EmptyTypes) is not null)
		{
			if (Activator.CreateInstance(viewType) is Control control)
			{
				control.DataContext = viewModel;
				return control;
			}
		}

		if (serviceProvider.GetRequiredService(viewType) is Control instance)
		{
			instance.DataContext = viewModel;
			return instance;
		}

		throw new NullReferenceException($"The Class {viewType.FullName} is not a Control.");
	}

	public async Task<bool> CanNavigateAsync(Control? currentView)
	{
		if (currentView?.DataContext is INavigationGuard navigationGuard)
		{
			return await navigationGuard.CanNavigateAsync().ConfigureAwait(false);
		}

		return true;
	}

	public async Task DisposeView(Control? control)
	{
		await DisposeObject(control);
		await DisposeObject(control?.DataContext);
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
