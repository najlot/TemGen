using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Client.MVVM;
using Todo.Client.MVVM.Services;
using Todo.Client.MVVM.ViewModel;
using Todo.ClientBase.Validation;

namespace Todo.ClientBase.ViewModel;

public class ChecklistTaskViewModel : AbstractValidationViewModel
{
	private readonly IErrorService _errorService;
	private readonly INavigationService _navigationService;

	private int _id;
	public int Id { get => _id; set => Set(ref _id, value); }

	private bool _isDone;
	public bool IsDone { get => _isDone; set => Set(ref _isDone, value); }

	private string _description = string.Empty;
	public string Description { get => _description; set => Set(ref _description, value); }

	private Guid _parentId;
	public Guid ParentId { get => _parentId; set => Set(nameof(ParentId), ref _parentId, value); }

	public ChecklistTaskViewModel(
		IErrorService errorService,
		INavigationService navigationService)
	{
		_errorService = errorService;
		_navigationService = navigationService;

		SaveCommand = new AsyncCommand(RequestSave, DisplayError);
		DeleteCommand = new AsyncCommand(RequestDelete, DisplayError);

		SetValidation(new ChecklistTaskValidationList());
	}

	private async Task DisplayError(Task task)
	{
		await _errorService.ShowAlertAsync("Error...", task.Exception);
	}

	private readonly List<Func<ChecklistTaskViewModel, Task>> _onSaveRequested = [];
	public void OnSaveRequested(Func<ChecklistTaskViewModel, Task> func) => _onSaveRequested.Add(func);

	public AsyncCommand SaveCommand { get; }
	private async Task RequestSave()
	{
		foreach (var func in _onSaveRequested)
		{
			await func(this);
		}
	}

	private readonly List<Func<ChecklistTaskViewModel, Task<bool>>> _onDeleteRequested = [];
	public void OnDeleteRequested(Func<ChecklistTaskViewModel, Task<bool>> func) => _onDeleteRequested.Add(func);

	public AsyncCommand DeleteCommand { get; }
	private async Task RequestDelete()
	{
		bool navigateBack = true;

		foreach (var func in _onDeleteRequested)
		{
			if (!await func(this))
			{
				navigateBack = false;
			}
		}

		if (navigateBack)
		{
			await _navigationService.NavigateBack();
		}
	}
}