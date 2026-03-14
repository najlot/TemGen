using System;
using System.Collections.Generic;
using System.Linq;
<#cs if(Entries.Where(e => e.IsArray).Any()) { WriteLine("using System.Threading;"); } #>using System.Threading.Tasks;
<#cs if(Entries.Where(e => e.IsArray).Any()) { WriteLine("using System.Windows.Input;"); } #>using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Client.Data.Services;
using <#cs Write(Project.Namespace)#>.Client.Localisation;
using <#cs Write(Project.Namespace)#>.Client.MVVM;
using <#cs Write(Project.Namespace)#>.Contracts;
using <#cs Write(Project.Namespace)#>.Contracts.Events;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

public <#cs if(Entries.Where(e => e.IsArray).Any()) Write("partial ");#>class <#cs Write(Definition.Name)#>ViewModel : ValidationViewModelBase, IParameterizable, IAsyncInitializable, INavigationGuard, IDisposable
{
	private readonly I<#cs Write(Definition.Name)#>Service _<#cs Write(Definition.NameLow)#>Service;
<#cs
var references = Entries
	.Where(e => e.IsReference)
	.Select(e => e.ReferenceType)
	.Distinct()
	.Select(n => Definitions.First(d => d.Name == n))
	.ToArray();

foreach (var definition in references)
{
	WriteLine($"	private readonly I{definition.Name}Service _{definition.NameLow}Service;");
}

WriteLine("");

foreach (var definition in references)
{
	WriteLine($"	public IEnumerable<{definition.Name}ListItemModel> Available{definition.Name}s {{ get; set => Set(ref field, value); }} = [];");
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
		WriteLine($"	public {entry.EntryType}[] Available{entry.EntryType}s {{ get; }} = Enum.GetValues<{entry.EntryType}>();");
	}
}
#>
	public Guid Id { get; set => Set(ref field, value); }
<#cs
foreach (var entry in Entries.Where(e => !e.IsArray))
{
	var defaultValue = "";
	if (!entry.IsNullable && entry.EntryType.ToLower() == "string")
	{
		defaultValue = " = string.Empty;";
	}

	var suffix = "";

	if (entry.IsReference)
	{
		suffix = "Id";
	}

	WriteLine($"	public {entry.EntryType} {entry.Field}{suffix} {{ get; set => Set(ref field, value); }}{defaultValue}");
}
#>
	private readonly ChangeTracker _changeTracker = new();

	public bool CanUndo => _changeTracker.CanUndo;
	public bool CanRedo => _changeTracker.CanRedo;

	public bool IsBusy { get; private set => Set(ref field, value); }
	public bool CanEdit { get; private set => Set(ref field, value); } = true;

	public bool IsNew { get; set; }

	public <#cs Write(Definition.Name)#>ViewModel(
		I<#cs Write(Definition.Name)#>Service <#cs Write(Definition.NameLow)#>Service,
<#cs
var references = Entries
	.Where(e => e.IsReference)
	.Select(e => e.ReferenceType)
	.Distinct()
	.Select(n => Definitions.First(d => d.Name == n))
	.ToArray();

foreach (var definition in references)
{
	WriteLine($"		I{definition.Name}Service {definition.NameLow}Service,");
}
#>		ViewModelBaseParameters<<#cs Write(Definition.Name)#>ViewModel> parameters) : base(parameters)
	{
		_<#cs Write(Definition.NameLow)#>Service = <#cs Write(Definition.NameLow)#>Service;
<#cs
var references = Entries
	.Where(e => e.IsReference)
	.Select(e => e.ReferenceType)
	.Distinct()
	.Select(n => Definitions.First(d => d.Name == n))
	.ToArray();

foreach (var definition in references)
{
	WriteLine($"		_{definition.NameLow}Service = {definition.NameLow}Service;");
}
#>
		NavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));
		SaveCommand = new AsyncCommand(SaveAsync, t => HandleError(t.Exception)<#cs if(Entries.Where(e => e.IsArray).Any()) Write(", () => CanUndo && !HasErrors"); else Write(", () => CanUndo"); #>);
		DeleteCommand = new AsyncCommand(DeleteAsync, t => HandleError(t.Exception));
		UndoCommand = new RelayCommand(() => _changeTracker.Undo(), () => _changeTracker.CanUndo);
		RedoCommand = new RelayCommand(() => _changeTracker.Redo(), () => _changeTracker.CanRedo);

		ChangeVisitor = _changeTracker;
		_changeTracker.StateChanged += () =>
		{
			RaisePropertiesChanged(nameof(CanUndo), nameof(CanRedo));
			UndoCommand.RaiseCanExecuteChanged();
			RedoCommand.RaiseCanExecuteChanged();
			SaveCommand.RaiseCanExecuteChanged();
		};
<#cs if(Entries.Where(e => e.IsArray).Any()) { WriteLine("");  WriteLine("\t\tHasErrorsChanged += SaveCommand.RaiseCanExecuteChanged;"); } #>
		_<#cs Write(Definition.NameLow)#>Service.ItemUpdated += Handle;
	}

	public void SetParameters(IReadOnlyDictionary<string, object> parameters)
	{
		if (parameters.TryGetValue("Id", out var idObj) && idObj is Guid id)
		{
			Id = id;
		}
	}

	public async Task InitializeAsync()
	{
		if (Id == Guid.Empty)
		{
			var model = _<#cs Write(Definition.NameLow)#>Service.Create<#cs Write(Definition.Name)#>();
			Map.From(model).To(this);
			IsNew = true;
		}
		else
		{
			var model = await _<#cs Write(Definition.NameLow)#>Service.GetItemAsync(Id);
			Map.From(model).To(this);
			IsNew = false;
		}

<#cs
var references = Entries
	.Where(e => e.IsReference)
	.Select(e => e.ReferenceType)
	.Distinct()
	.Select(n => Definitions.First(d => d.Name == n))
	.ToArray();

foreach (var definition in references)
{
	WriteLine($"		Available{definition.Name}s = await _{definition.NameLow}Service.GetItemsAsync();");
}

if (references.Length > 0) WriteLine("");
#>		await _<#cs Write(Definition.NameLow)#>Service.StartEventListener();

		_changeTracker.Clear();
<#cs
foreach (var entry in Entries.Where(e => e.IsArray))
{
	WriteLine($"		Initialize{entry.Field}Tracking();");
}
#>	}

	private async Task Handle(object? sender, <#cs Write(Definition.Name)#>Updated obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			if (Id != obj.Id)
			{
				return;
			}

			Map.From(obj).To(this);
			_changeTracker.Clear();
<#cs
foreach (var entry in Entries.Where(e => e.IsArray))
{
	WriteLine($"			Initialize{entry.Field}Tracking();");
}
#>		});

	public AsyncCommand NavigateBackCommand { get; }
	public AsyncCommand SaveCommand { get; }
	public RelayCommand UndoCommand { get; }
	public RelayCommand RedoCommand { get; }

	private async Task<bool> SaveAsync()
	{
		if (IsBusy)
		{
			return false;
		}

		try
		{
			IsBusy = true;

			ValidateAll();
			if (HasErrors)
			{
				return false;
			}

			var model = Map.From(this).To<<#cs Write(Definition.Name)#>Model>();

			if (IsNew)
			{
				await _<#cs Write(Definition.NameLow)#>Service.AddItemAsync(model);
				IsNew = false;
			}
			else
			{
				await _<#cs Write(Definition.NameLow)#>Service.UpdateItemAsync(model);
			}

			_changeTracker.Clear();
			return true;
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync($"{ErrorLoc.ErrorSaving} {ex.Message}");
			return false;
		}
		finally
		{
			IsBusy = false;
		}
	}

	public DeleteConfirmationDialogViewModel DeleteDialogViewModel { get; } = new DeleteConfirmationDialogViewModel();

	public AsyncCommand DeleteCommand { get; }
	public async Task DeleteAsync()
	{
		if (IsBusy)
		{
			return;
		}

		if (DeleteDialogViewModel.IsVisible)
		{
			return;
		}

		CanEdit = false;

		try
		{
			var deleteDialogResult = await DeleteDialogViewModel.ShouldDelete();
			if (deleteDialogResult == DeleteDialogResult.Cancel)
			{
				return;
			}
		}
		finally
		{
			CanEdit = true;
		}

		try
		{
			IsBusy = true;

			await _<#cs Write(Definition.NameLow)#>Service.DeleteItemAsync(Id);
			_changeTracker.Clear();
			await NavigationService.NavigateBack();
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync($"{ErrorLoc.ErrorDeleting} {ex.Message}");
		}
		finally
		{
			IsBusy = false;
		}
	}

	public SaveConfirmationDialogViewModel SaveDialogViewModel { get; } = new SaveConfirmationDialogViewModel();
	
	public async Task<bool> CanNavigateAsync()
	{
		if (SaveDialogViewModel.IsVisible || DeleteDialogViewModel.IsVisible)
		{
			return false;
		}

		if (_changeTracker.CanUndo)
		{
			CanEdit = false;

			try
			{
				SaveDialogViewModel.CanSave = !HasErrors;
				var saveDialogResult = await SaveDialogViewModel.ShouldSave();
				switch (saveDialogResult)
				{
					case SaveDialogResult.Save:
						return await SaveAsync();
					case SaveDialogResult.Discard:
						// Nothing to do, just continue navigation
						break;
					case SaveDialogResult.Cancel:
						return false;
				}
			}
			finally
			{
				CanEdit = true;
			}
		}

		return true;
	}

	protected override IEnumerable<ValidationResult> Validate(string? propertyName)
	{
		return [];
	}

	protected override bool ShouldIgnorePropertyForChangesAndValidation(string? propertyName)
		=> base.ShouldIgnorePropertyForChangesAndValidation(propertyName)
			|| propertyName is nameof(IsBusy)
							or nameof(IsNew)
							or nameof(CanEdit)
							or nameof(Id)<#cs
var ignoreReferences = Entries
	.Where(e => e.IsReference)
	.Select(e => e.ReferenceType)
	.Distinct()
	.Select(n => Definitions.First(d => d.Name == n))
	.ToArray();

foreach (var definition in ignoreReferences)
{
	Write($"\r\n\t\t\t\t\t\t\tor nameof(Available{definition.Name}s)");
}

foreach (var entry in Entries.Where(e => e.IsEnumeration))
{
	Write($"\r\n\t\t\t\t\t\t\tor nameof(Available{entry.EntryType}s)");
}
#>;

	private bool _disposedValue;
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				_<#cs Write(Definition.NameLow)#>Service.ItemUpdated -= Handle;
			}

			_disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>
