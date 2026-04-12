using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Todo.Client.Data.TodoItems;
using Todo.Client.Data.Users;
using Todo.Client.Localisation;
using Todo.Client.MVVM;
using Todo.ClientBase.History;
using Todo.ClientBase.Shared;
using Todo.Contracts.TodoItems;

namespace Todo.ClientBase.TodoItems;

public partial class TodoItemViewModel : ValidationViewModelBase, IParameterizable, IAsyncInitializable, INavigationGuard, IDisposable
{
	private readonly ITodoItemService _todoItemService;
	private readonly IUserService _userService;

	public IEnumerable<UserListItemModel> AvailableUsers { get; set => Set(ref field, value); } = [];

	public TodoItemStatus[] AvailableTodoItemStatus { get; } = Enum.GetValues<TodoItemStatus>();

	public Guid Id { get; set => Set(ref field, value); }
	public string Title { get; set => Set(ref field, value); } = string.Empty;
	public string Content { get; set => Set(ref field, value); } = string.Empty;

	public DateTime CreatedAt
	{
		get;
		set
		{
			if (Set(ref field, value))
			{
				RaisePropertiesChanged(nameof(CreatedAtDate), nameof(CreatedAtTime));
			}
		}
	}

	public DateTimeOffset? CreatedAtDate
	{
		get => ToDateTimeOffset(CreatedAt);
		set
		{
			if (value is null)
			{
				return;
			}

			CreatedAt = CombineDateAndTime(value.Value.Date, CreatedAt.TimeOfDay);
		}
	}

	public TimeSpan? CreatedAtTime
	{
		get => CreatedAt.TimeOfDay;
		set
		{
			if (value is null)
			{
				return;
			}

			CreatedAt = CombineDateAndTime(CreatedAt.Date, value);
		}
	}
	public string CreatedBy { get; set => Set(ref field, value); } = string.Empty;
	public Guid AssignedToId { get; set => Set(ref field, value); }
	public TodoItemStatus Status { get; set => Set(ref field, value); }

	public DateTime ChangedAt
	{
		get;
		set
		{
			if (Set(ref field, value))
			{
				RaisePropertiesChanged(nameof(ChangedAtDate), nameof(ChangedAtTime));
			}
		}
	}

	public DateTimeOffset? ChangedAtDate
	{
		get => ToDateTimeOffset(ChangedAt);
		set
		{
			if (value is null)
			{
				return;
			}

			ChangedAt = CombineDateAndTime(value.Value.Date, ChangedAt.TimeOfDay);
		}
	}

	public TimeSpan? ChangedAtTime
	{
		get => ChangedAt.TimeOfDay;
		set
		{
			if (value is null)
			{
				return;
			}

			ChangedAt = CombineDateAndTime(ChangedAt.Date, value);
		}
	}
	public string ChangedBy { get; set => Set(ref field, value); } = string.Empty;
	public string Priority { get; set => Set(ref field, value); } = string.Empty;

	private static DateTime CombineDateAndTime(DateTime datePart, TimeSpan? timePart)
		=> datePart.Date + (timePart ?? TimeSpan.Zero);

	private static DateTimeOffset? ToDateTimeOffset(DateTime? value)
		=> value is null ? null : new DateTimeOffset(DateTime.SpecifyKind(value.Value, DateTimeKind.Unspecified), TimeSpan.Zero);


	private readonly ChangeTracker _changeTracker = new();

	public bool CanNavigateToHistory => !IsNew;

	public bool IsBusy { get; private set => Set(ref field, value); }
	public bool CanEdit { get; private set => Set(ref field, value); } = true;

	public bool IsNew
	{
		get;
		set => Set(ref field, value, () =>
		{
			RaisePropertyChanged(nameof(CanNavigateToHistory));
			NavigateToHistoryCommand.RaiseCanExecuteChanged();
		});
	}

	public TodoItemViewModel(
		ITodoItemService todoItemService,
		IUserService userService,
		ViewModelBaseParameters<TodoItemViewModel> parameters) : base(parameters)
	{
		_todoItemService = todoItemService;
		_userService = userService;

		NavigateBackCommand = new AsyncCommand(NavigationService.NavigateBack, t => HandleError(t.Exception));
		NavigateToHistoryCommand = new AsyncCommand(NavigateToHistoryAsync, t => HandleError(t.Exception), () => CanNavigateToHistory);
		SaveCommand = new AsyncCommand(SaveAsync, t => HandleError(t.Exception), () => !HasErrors);
		DeleteCommand = new AsyncCommand(DeleteAsync, t => HandleError(t.Exception));
		UndoCommand = new RelayCommand(_changeTracker.Undo, () => _changeTracker.CanUndo);
		RedoCommand = new RelayCommand(_changeTracker.Redo, () => _changeTracker.CanRedo);

		ChangeVisitor = _changeTracker;

		_changeTracker.StateChanged += UndoCommand.RaiseCanExecuteChanged;
		_changeTracker.StateChanged += RedoCommand.RaiseCanExecuteChanged;

		HasErrorsChanged += SaveCommand.RaiseCanExecuteChanged;

		_todoItemService.ItemUpdated += Handle;
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
			var model = _todoItemService.CreateTodoItem();
			Map.From(model).To(this);
			IsNew = true;
		}
		else
		{
			var model = await _todoItemService.GetItemAsync(Id);
			Map.From(model).To(this);
			IsNew = false;
		}

		AvailableUsers = await _userService.GetItemsAsync();

		await _todoItemService.StartEventListener();

		_changeTracker.Clear();

		InitializeChecklistTracking();
	}

	private async Task Handle(object? sender, TodoItemUpdated obj)
	{
		if (Id != obj.Id)
		{
			return;
		}

		await DispatcherHelper.InvokeOnUIThread(() =>
		{
			Map.From(obj).To(this);
			_changeTracker.Clear();

			InitializeChecklistTracking();

		});
	}

	public AsyncCommand NavigateBackCommand { get; }
	public AsyncCommand NavigateToHistoryCommand { get; }
	public AsyncCommand SaveCommand { get; }
	public RelayCommand UndoCommand { get; }
	public RelayCommand RedoCommand { get; }

	private async Task NavigateToHistoryAsync()
	{
		if (!CanNavigateToHistory)
		{
			return;
		}

		await NavigationService.NavigateForward<EntityHistoryViewModel>(new()
		{
			{ "EntityId", Id },
			{ "EntityName", "TodoItem" }
		});
	}

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

			var model = Map.From(this).To<TodoItemModel>();

			if (IsNew)
			{
				await _todoItemService.AddItemAsync(model);
				IsNew = false;
			}
			else
			{
				await _todoItemService.UpdateItemAsync(model);
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

			await _todoItemService.DeleteItemAsync(Id);
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
							or nameof(CanNavigateToHistory)
							or nameof(Id)
							or nameof(AvailableUsers)
							or nameof(AvailableTodoItemStatus)
;

	private bool _disposedValue;
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				_changeTracker.StateChanged -= UndoCommand.RaiseCanExecuteChanged;
				_changeTracker.StateChanged -= RedoCommand.RaiseCanExecuteChanged;
				HasErrorsChanged -= SaveCommand.RaiseCanExecuteChanged;
				_todoItemService.ItemUpdated -= Handle;
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
