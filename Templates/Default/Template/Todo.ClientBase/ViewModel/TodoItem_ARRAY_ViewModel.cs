using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.MVVM;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

public class <#cs Write(Definition.Name)#>ViewModel : ValidationViewModelBase
{
	public int Id { get; set => Set(ref field, value); }

<#cs
foreach (var entry in Entries.Where(e => !e.IsArray))
{
	var defaultValue = "";
	if (!entry.IsNullable && entry.EntryType.ToLower() == "string")
	{
		defaultValue = " = string.Empty;";
	}

	WriteLine($"	public {entry.EntryType} {entry.Field} {{ get; set => Set(ref field, value); }}{defaultValue}");
}
#>
	public Guid ParentId { get; set => Set( ref field, value); }

	public <#cs Write(Definition.Name)#>ViewModel(ViewModelBaseParameters<<#cs Write(Definition.Name)#>ViewModel> parameters) : base(parameters)
	{
		SaveCommand = new AsyncCommand(RequestSave, t => HandleError(t.Exception));
		DeleteCommand = new AsyncCommand(RequestDelete, t => HandleError(t.Exception));
	}

	private readonly List<Func<<#cs Write(Definition.Name)#>ViewModel, Task>> _onSaveRequested = [];
	public void OnSaveRequested(Func<<#cs Write(Definition.Name)#>ViewModel, Task> func) => _onSaveRequested.Add(func);

	public AsyncCommand SaveCommand { get; }
	private async Task RequestSave()
	{
		foreach (var func in _onSaveRequested)
		{
			await func(this);
		}
	}

	private readonly List<Func<<#cs Write(Definition.Name)#>ViewModel, Task<bool>>> _onDeleteRequested = [];
	public void OnDeleteRequested(Func<<#cs Write(Definition.Name)#>ViewModel, Task<bool>> func) => _onDeleteRequested.Add(func);

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
			|| propertyName is nameof(Id) or nameof(ParentId);
}<#cs
SetOutputPath(!Definition.IsArray);
RelativePath = RelativePath.Replace("_ARRAY_", "");
#>