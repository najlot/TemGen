using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Najlot.Map;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Client.Data.Services;
using <#cs Write(Project.Namespace)#>.Client.MVVM;
using <#cs Write(Project.Namespace)#>.Client.MVVM.Services;
using <#cs Write(Project.Namespace)#>.Client.MVVM.Validation;
using <#cs Write(Project.Namespace)#>.Client.MVVM.ViewModel;
using <#cs Write(Project.Namespace)#>.ClientBase.Validation;
using <#cs Write(Project.Namespace)#>.Contracts;
using <#cs Write(Project.Namespace)#>.Contracts.Events;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

public <#cs if(Entries.Where(e => e.IsArray).Any()) Write("partial ");#>class <#cs Write(Definition.Name)#>ViewModel : AbstractValidationViewModel, IDisposable
{
	private readonly IErrorService _errorService;
	private readonly INavigationService _navigationService;
	private readonly I<#cs Write(Definition.Name)#>Service _<#cs Write(Definition.NameLow)#>Service;
	private readonly IMessenger _messenger;
	private readonly IMap _map;
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
#>
	private Guid _id;
	public Guid Id { get => _id; set => Set(ref _id, value); }
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
	private bool _isBusy;
	public bool IsBusy { get => _isBusy; private set => Set(ref _isBusy, value); }

	public bool IsNew { get; set; }

	public <#cs Write(Definition.Name)#>ViewModel(
		IErrorService errorService,
		INavigationService navigationService,
		I<#cs Write(Definition.Name)#>Service <#cs Write(Definition.NameLow)#>Service,
		IMessenger messenger,
		IMap map)
	{
		_errorService = errorService;
		_navigationService = navigationService;
		_<#cs Write(Definition.NameLow)#>Service = <#cs Write(Definition.NameLow)#>Service;
		_messenger = messenger;
		_map = map;

		SaveCommand = new AsyncCommand(SaveAsync, DisplayError);
		DeleteCommand = new AsyncCommand(DeleteAsync, DisplayError);

		SetValidation(new <#cs Write(Definition.Name)#>ValidationList());

		_messenger.Register<<#cs Write(Definition.Name)#>Updated>(Handle);
	}

	private async Task DisplayError(Task task)
	{
		await _errorService.ShowAlertAsync("Error...", task.Exception);
	}

	public void Handle(<#cs Write(Definition.Name)#>Updated obj)
	{
		if (Id != obj.Id)
		{
			return;
		}

		_map.From(obj).To(this);
	}

	public AsyncCommand SaveCommand { get; }
	public async Task SaveAsync()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;

			var errors = Errors
				.Where(err => err.Severity > ValidationSeverity.Info)
				.Select(e => e.Text);

			if (errors.Any())
			{
				var message = "There are some validation errors:";
				message += Environment.NewLine + Environment.NewLine;
				message += string.Join(Environment.NewLine, errors);
				message += Environment.NewLine + Environment.NewLine;
				message += "Do you want to continue?";

				var vm = new YesNoPageViewModel()
				{
					Title = "Validation",
					Message = message
				};

				var selection = await _navigationService.RequestInputAsync(vm);

				if (!selection)
				{
					return;
				}
			}

			await _navigationService.NavigateBack();

			var model = _map.From(this).To<<#cs Write(Definition.Name)#>Model>();

			if (IsNew)
			{
				await _<#cs Write(Definition.NameLow)#>Service.AddItemAsync(model);
				IsNew = false;
			}
			else
			{
				await _<#cs Write(Definition.NameLow)#>Service.UpdateItemAsync(model);
			}
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error saving...", ex);
		}
		finally
		{
			IsBusy = false;
		}
	}

	public AsyncCommand DeleteCommand { get; }
	public async Task DeleteAsync()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;

			var vm = new YesNoPageViewModel()
			{
				Title = "Delete?",
				Message = "Should the item be deleted?"
			};

			var selection = await _navigationService.RequestInputAsync(vm);

			if (selection)
			{
				await _navigationService.NavigateBack();
				await _<#cs Write(Definition.NameLow)#>Service.DeleteItemAsync(Id);
			}
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error deleting...", ex);
		}
		finally
		{
			IsBusy = false;
		}
	}

	public void Dispose() => _messenger.Unregister(this);
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>