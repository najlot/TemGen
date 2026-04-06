using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Client.MVVM;

namespace Todo.ClientBase.ViewModels;

public class ChecklistTaskViewModel : ValidationViewModelBase
{
	public int Id { get; set => Set(ref field, value); }

	public bool IsDone { get; set => Set(ref field, value); }
	public string Description { get; set => Set(ref field, value); } = string.Empty;



	public Guid ParentId { get; set => Set( ref field, value); }
	public bool IsBusy { get; set => Set(ref field, value); }

	public ChecklistTaskViewModel(ViewModelBaseParameters<ChecklistTaskViewModel> parameters) : base(parameters)
	{
		SaveCommand = new AsyncCommand(RequestSave, t => HandleError(t.Exception));
		DeleteCommand = new AsyncCommand(RequestDelete, t => HandleError(t.Exception));
	}

	public void InitializeTracking(ChangeTracker changeTracker)
	{
		ChangeVisitor = changeTracker;

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
			await NavigationService.NavigateBack();
		}
	}

	protected override IEnumerable<ValidationResult> Validate(string? propertyName)
	{
		return [];
	}

	protected override bool ShouldIgnorePropertyForChangesAndValidation(string? propertyName)
		=> base.ShouldIgnorePropertyForChangesAndValidation(propertyName)
			|| propertyName is nameof(Id) or nameof(ParentId) or nameof(IsBusy);
}