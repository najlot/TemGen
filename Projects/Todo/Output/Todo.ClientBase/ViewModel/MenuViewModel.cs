using System;
using System.Threading.Tasks;
using Todo.Client.MVVM;
using Todo.Client.MVVM.Services;
using Todo.Client.MVVM.ViewModel;

namespace Todo.ClientBase.ViewModel;

public class MenuViewModel : AbstractViewModel
{
	private readonly IErrorService _errorService;
	private readonly INavigationService _navigationService;
	private bool _isBusy = false;

	private readonly AllNotesViewModel _allNotesViewModel;
	private readonly AllTodoItemsViewModel _allTodoItemsViewModel;

	public AsyncCommand NavigateToNotes { get; }
	public async Task NavigateToNotesAsync()
	{
		if (_isBusy)
		{
			return;
		}

		try
		{
			_isBusy = true;

			var refreshTask = _allNotesViewModel.RefreshNotesAsync();
			await _navigationService.NavigateForward(_allNotesViewModel);
			await refreshTask;
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Could not load...", ex);
		}
		finally
		{
			_isBusy = false;
		}
	}

	public AsyncCommand NavigateToTodoItems { get; }
	public async Task NavigateToTodoItemsAsync()
	{
		if (_isBusy)
		{
			return;
		}

		try
		{
			_isBusy = true;

			var refreshTask = _allTodoItemsViewModel.RefreshTodoItemsAsync();
			await _navigationService.NavigateForward(_allTodoItemsViewModel);
			await refreshTask;
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Could not load...", ex);
		}
		finally
		{
			_isBusy = false;
		}
	}

	public MenuViewModel(IErrorService errorService,
		AllNotesViewModel allNotesViewModel,
		AllTodoItemsViewModel allTodoItemsViewModel,
		INavigationService navigationService)
	{
		_errorService = errorService;
		_allNotesViewModel = allNotesViewModel;
		_allTodoItemsViewModel = allTodoItemsViewModel;
		_navigationService = navigationService;

		NavigateToNotes = new AsyncCommand(NavigateToNotesAsync, DisplayError);
		NavigateToTodoItems = new AsyncCommand(NavigateToTodoItemsAsync, DisplayError);
	}

	private async Task DisplayError(Task task)
	{
		await _errorService.ShowAlertAsync("Error...", task.Exception);
	}
}