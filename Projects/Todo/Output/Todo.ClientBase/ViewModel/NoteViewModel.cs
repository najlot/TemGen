using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Client.Data.Services;
using Todo.Client.Localisation;
using Todo.Client.MVVM;
using Todo.Contracts;
using Todo.Contracts.Events;

namespace Todo.ClientBase.ViewModel;

public class NoteViewModel : ValidationViewModelBase, IParameterizable, IAsyncInitializable, INavigationGuard, IDisposable
{
	private readonly INoteService _noteService;


	public PredefinedColor[] AvailablePredefinedColors { get; } = Enum.GetValues<PredefinedColor>();

	public Guid Id { get; set => Set(ref field, value); }
	public string Title { get; set => Set(ref field, value); } = string.Empty;
	public string Content { get; set => Set(ref field, value); } = string.Empty;
	public PredefinedColor Color { get; set => Set(ref field, value); }

	private readonly ChangeTracker _changeTracker = new();

	public bool CanUndo => _changeTracker.CanUndo;
	public bool CanRedo => _changeTracker.CanRedo;

	public bool IsBusy { get; private set => Set(ref field, value); }
	public bool CanEdit { get; private set => Set(ref field, value); } = true;

	public bool IsNew { get; set; }

	public NoteViewModel(
		INoteService noteService,
		ViewModelBaseParameters<NoteViewModel> parameters) : base(parameters)
	{
		_noteService = noteService;

		NavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));
		SaveCommand = new AsyncCommand(SaveAsync, t => HandleError(t.Exception), () => CanUndo);
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

		_noteService.ItemUpdated += Handle;
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
			var model = _noteService.CreateNote();
			Map.From(model).To(this);
			IsNew = true;
		}
		else
		{
			var model = await _noteService.GetItemAsync(Id);
			Map.From(model).To(this);
			IsNew = false;
		}

		await _noteService.StartEventListener();

		_changeTracker.Clear();

	}

	private async Task Handle(object? sender, NoteUpdated obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			if (Id != obj.Id)
			{
				return;
			}

			Map.From(obj).To(this);
			_changeTracker.Clear();

		});

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

			var model = Map.From(this).To<NoteModel>();

			if (IsNew)
			{
				await _noteService.AddItemAsync(model);
				IsNew = false;
			}
			else
			{
				await _noteService.UpdateItemAsync(model);
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

			await _noteService.DeleteItemAsync(Id);
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
							or nameof(AvailablePredefinedColors)
;

	private bool _disposedValue;
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				_noteService.ItemUpdated -= Handle;
			}

			_disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
