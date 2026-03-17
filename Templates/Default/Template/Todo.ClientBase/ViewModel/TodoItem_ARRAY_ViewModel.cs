using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.MVVM;

namespace <# Project.Namespace#>.ClientBase.ViewModel;

public class <# Definition.Name#>ViewModel : ValidationViewModelBase
{
	public int Id { get; set => Set(ref field, value); }

<#for entry in Entries.Where(e => !e.IsArray)
#>	public <# entry.EntryType#> <# entry.Field#> { get; set => Set(ref field, value); }<#if !entry.IsNullable && entry.EntryType.ToLower() == "string"
#> = string.Empty;<#end#>
<#end#>
	public Guid ParentId { get; set => Set( ref field, value); }

	public <# Definition.Name#>ViewModel(ViewModelBaseParameters<<# Definition.Name#>ViewModel> parameters) : base(parameters)
	{
		SaveCommand = new AsyncCommand(RequestSave, t => HandleError(t.Exception));
		DeleteCommand = new AsyncCommand(RequestDelete, t => HandleError(t.Exception));
	}

	private readonly List<Func<<# Definition.Name#>ViewModel, Task>> _onSaveRequested = [];
	public void OnSaveRequested(Func<<# Definition.Name#>ViewModel, Task> func) => _onSaveRequested.Add(func);

	public AsyncCommand SaveCommand { get; }
	private async Task RequestSave()
	{
		foreach (var func in _onSaveRequested)
		{
			await func(this);
		}
	}

	private readonly List<Func<<# Definition.Name#>ViewModel, Task<bool>>> _onDeleteRequested = [];
	public void OnDeleteRequested(Func<<# Definition.Name#>ViewModel, Task<bool>> func) => _onDeleteRequested.Add(func);

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