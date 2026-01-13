using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.MVVM;
using <#cs Write(Project.Namespace)#>.Client.MVVM.Services;
using <#cs Write(Project.Namespace)#>.Client.MVVM.ViewModel;
using <#cs Write(Project.Namespace)#>.ClientBase.Validation;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

public class <#cs Write(Definition.Name)#>ViewModel : AbstractValidationViewModel
{
	private readonly IErrorService _errorService;
	private readonly INavigationService _navigationService;
<#cs
var references = Entries
	.Where(e => e.IsReference)
	.Select(e => e.ReferenceType)
	.Distinct()
	.Select(n => Definitions.First(d => d.Name == n))
	.ToArray();

foreach (var definition in references)
{
	WriteLine($"	private IEnumerable<{definition.Name}ListItemModel> _available{definition.Name}s;");
}

WriteLine("");

foreach (var definition in references)
{
	WriteLine($"	public IEnumerable<{definition.Name}ListItemModel> Available{definition.Name}s {{ get => _available{definition.Name}s; set => Set(nameof(Available{definition.Name}s), ref _available{definition.Name}s, value); }}");
}

foreach(var entry in Entries.Where(e => e.IsEnumeration))
{
	if (entry.IsNullable)
	{
		WriteLine("");
		WriteLine($"	private static IEnumerable<{entry.EntryType}?> GetAvailable{entry.EntryType}s()");
		WriteLine("	{");
		WriteLine("		yield return null;");
		WriteLine("");
		WriteLine($"		foreach (var entry in Enum.GetValues(typeof({entry.EntryType})) as {entry.EntryType}[])");
		WriteLine("		{");
		WriteLine("			yield return entry;");
		WriteLine("		}");
		WriteLine("	}");
		WriteLine("");
		WriteLine($"	public List<{entry.EntryType}?> Available{entry.EntryType}s {{ get; }} = GetAvailable{entry.EntryType}s().ToList();");
	}
	else
	{
		WriteLine($"	public List<{entry.EntryType}> Available{entry.EntryType}s {{ get; }} = new(Enum.GetValues(typeof({entry.EntryType})) as {entry.EntryType}[]);");
	}
}
#>	private int _id;
	public int Id { get => _id; set => Set(ref _id, value); }
<#cs
foreach (var entry in Entries.Where(e => !e.IsArray))
{
	WriteLine("");

	var defaultValue = "";
	if (!entry.IsNullable && entry.EntryType.ToLower() == "string")
	{
		defaultValue = " = string.Empty";
	}

	WriteLine($"	private {entry.EntryType} _{entry.FieldLow}{defaultValue};");
	WriteLine($"	public {entry.EntryType} {entry.Field} {{ get => _{entry.FieldLow}; set => Set(ref _{entry.FieldLow}, value); }}");
}
#>
	private Guid _parentId;
	public Guid ParentId { get => _parentId; set => Set(nameof(ParentId), ref _parentId, value); }

	public <#cs Write(Definition.Name)#>ViewModel(
		IErrorService errorService,
		INavigationService navigationService)
	{
		_errorService = errorService;
		_navigationService = navigationService;

		SaveCommand = new AsyncCommand(RequestSave, DisplayError);
		DeleteCommand = new AsyncCommand(RequestDelete, DisplayError);

		SetValidation(new <#cs Write(Definition.Name)#>ValidationList());
	}

	private async Task DisplayError(Task task)
	{
		await _errorService.ShowAlertAsync("Error...", task.Exception);
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
			await _navigationService.NavigateBack();
		}
	}
}<#cs
SetOutputPath(!Definition.IsArray);
RelativePath = RelativePath.Replace("_ARRAY_", "");
#>