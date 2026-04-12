using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Client.MVVM;

namespace Todo.ClientBase.TodoItems;

public class ChecklistTaskViewModel(ViewModelBaseParameters<ChecklistTaskViewModel> parameters)
	: ValidationViewModelBase(parameters)
{
	public int Id { get; set => Set(ref field, value); }

	public bool IsDone { get; set => Set(ref field, value); }
	public string Description { get; set => Set(ref field, value); } = string.Empty;

	public void InitializeTracking(ChangeTracker changeTracker)
	{
		ChangeVisitor = changeTracker;
	}

	protected override IEnumerable<ValidationResult> Validate(string? propertyName)
	{
		return [];
	}

	protected override bool ShouldIgnorePropertyForChangesAndValidation(string? propertyName)
		=> base.ShouldIgnorePropertyForChangesAndValidation(propertyName)
			|| propertyName is nameof(Id);
}