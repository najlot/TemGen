using System;
using System.Collections.Generic;
using System.Linq;
<#if Entries.Where(e => e.IsArray).Any()
#>using System.Threading;
using System.Windows.Input;
<#end#>using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.Client.Data.Services;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Client.MVVM;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Contracts.Events;

namespace <# Project.Namespace#>.ClientBase.ViewModel;

public <#if Entries.Where(e => e.IsArray).Any()
#>partial <#end#>class <# Definition.Name#>ViewModel : ValidationViewModelBase, IParameterizable, IAsyncInitializable, INavigationGuard, IDisposable
{
	private readonly I<# Definition.Name#>Service _<# Definition.NameLow#>Service;
<#for definition in Entries
	.Where(e => e.IsReference)
	.Select(e => e.ReferenceType)
	.Distinct()
	.Select(n => Definitions.First(d => d.Name == n))
#>	private readonly I<# definition.Name#>Service _<# definition.NameLow#>Service;
<#end#>
<#for definition in Entries
	.Where(e => e.IsReference)
	.Select(e => e.ReferenceType)
	.Distinct()
	.Select(n => Definitions.First(d => d.Name == n))
#>	public IEnumerable<<# definition.Name#>ListItemModel> Available<# definition.Name#>s { get; set => Set(ref field, value); } = [];
<#end#>
<#for entry in Entries.Where(e => e.IsEnumeration)
#><#if entry.IsNullable
#>	private static IEnumerable<<# entry.EntryType#>?> GetAvailable<# entry.EntryType#>s()
	{
		yield return null;

		foreach (var entry in Enum.GetValues(typeof(<# entry.EntryType#>)) as <# entry.EntryType#>[])
		{
			yield return entry;
		}
	}

	public List<<# entry.EntryType#>?> Available<# entry.EntryType#>s { get; } = GetAvailable<# entry.EntryType#>s().ToList();
<#else
#>	public <# entry.EntryType#>[] Available<# entry.EntryType#>s { get; } = Enum.GetValues<<# entry.EntryType#>>();
<#end#><#end#>
	public Guid Id { get; set => Set(ref field, value); }
<#for entry in Entries.Where(e => !e.IsArray)
#>	public <# entry.EntryType#> <# entry.Field#><#if entry.IsReference
#>Id<#end#> { get; set => Set(ref field, value); }<#if !entry.IsNullable && entry.EntryType.ToLower() == "string"
#> = string.Empty;<#end#>
<#end#>
	private readonly ChangeTracker _changeTracker = new();

	public bool CanUndo => _changeTracker.CanUndo;
	public bool CanRedo => _changeTracker.CanRedo;

	public bool IsBusy { get; private set => Set(ref field, value); }
	public bool CanEdit { get; private set => Set(ref field, value); } = true;

	public bool IsNew { get; set; }

	public <# Definition.Name#>ViewModel(
		I<# Definition.Name#>Service <# Definition.NameLow#>Service,
<#for definition in Entries
	.Where(e => e.IsReference)
	.Select(e => e.ReferenceType)
	.Distinct()
	.Select(n => Definitions.First(d => d.Name == n))
#>		I<# definition.Name#>Service <# definition.NameLow#>Service,
<#end#>		ViewModelBaseParameters<<# Definition.Name#>ViewModel> parameters) : base(parameters)
	{
		_<# Definition.NameLow#>Service = <# Definition.NameLow#>Service;
<#for definition in Entries
	.Where(e => e.IsReference)
	.Select(e => e.ReferenceType)
	.Distinct()
	.Select(n => Definitions.First(d => d.Name == n))
#>		_<# definition.NameLow#>Service = <# definition.NameLow#>Service;
<#end#>
		NavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));
		SaveCommand = new AsyncCommand(SaveAsync, t => HandleError(t.Exception)<#if Entries.Where(e => e.IsArray).Any()
#>, () => CanUndo && !HasErrors<#else
#>, () => CanUndo<#end#>);
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
<#if Entries.Where(e => e.IsArray).Any()
#>
		HasErrorsChanged += SaveCommand.RaiseCanExecuteChanged;
<#end#>
		_<# Definition.NameLow#>Service.ItemUpdated += Handle;
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
			var model = _<# Definition.NameLow#>Service.Create<# Definition.Name#>();
			Map.From(model).To(this);
			IsNew = true;
		}
		else
		{
			var model = await _<# Definition.NameLow#>Service.GetItemAsync(Id);
			Map.From(model).To(this);
			IsNew = false;
		}

<#for definition in Entries
	.Where(e => e.IsReference)
	.Select(e => e.ReferenceType)
	.Distinct()
	.Select(n => Definitions.First(d => d.Name == n))
#>		Available<# definition.Name#>s = await _<# definition.NameLow#>Service.GetItemsAsync();
<#end#>		await _<# Definition.NameLow#>Service.StartEventListener();

		_changeTracker.Clear();
<#for entry in Entries.Where(e => e.IsArray)
#>		Initialize<# entry.Field#>Tracking();
<#end#>	}

	private async Task Handle(object? sender, <# Definition.Name#>Updated obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			if (Id != obj.Id)
			{
				return;
			}

			Map.From(obj).To(this);
			_changeTracker.Clear();
<#for entry in Entries.Where(e => e.IsArray)
#>			Initialize<# entry.Field#>Tracking();
<#end#>		});

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

			var model = Map.From(this).To<<# Definition.Name#>Model>();

			if (IsNew)
			{
				await _<# Definition.NameLow#>Service.AddItemAsync(model);
				IsNew = false;
			}
			else
			{
				await _<# Definition.NameLow#>Service.UpdateItemAsync(model);
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

			await _<# Definition.NameLow#>Service.DeleteItemAsync(Id);
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
							or nameof(Id)
<#for definition in Entries
	.Where(e => e.IsReference)
	.Select(e => e.ReferenceType)
	.Distinct()
	.Select(n => Definitions.First(d => d.Name == n))
#>							or nameof(Available<# definition.Name#>s)
<#end#><#for entry in Entries.Where(e => e.IsEnumeration)
#>							or nameof(Available<# entry.EntryType#>s)
<#end#>;

	private bool _disposedValue;
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				_<# Definition.NameLow#>Service.ItemUpdated -= Handle;
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
