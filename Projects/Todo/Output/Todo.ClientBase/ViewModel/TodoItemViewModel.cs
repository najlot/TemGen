using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Client.Data.Services;
using Todo.Client.MVVM;
using Todo.Contracts;
using Todo.Contracts.Events;

namespace Todo.ClientBase.ViewModel;

public partial class TodoItemViewModel : ValidationViewModelBase, IParameterizable, IAsyncInitializable, IDisposable
{
	private readonly ITodoItemService _todoItemService;
	private readonly IUserService _userService;

	public IEnumerable<UserListItemModel> AvailableUsers { get; set => Set(ref field, value); } = [];
	public TodoItemStatus[] AvailableTodoItemStatus { get; } = Enum.GetValues<TodoItemStatus>();

	public Guid Id { get; set => Set(ref field, value); }
	public string Title { get; set => Set(ref field, value); } = string.Empty;
	public string Content { get; set => Set(ref field, value); } = string.Empty;
	public DateTime CreatedAt { get; set => Set(ref field, value); }
	public string CreatedBy { get; set => Set(ref field, value); } = string.Empty;
	public Guid AssignedToId { get; set => Set(ref field, value); }
	public TodoItemStatus Status { get; set => Set(ref field, value); }
	public DateTime ChangedAt { get; set => Set(ref field, value); }
	public string ChangedBy { get; set => Set(ref field, value); } = string.Empty;
	public string Priority { get; set => Set(ref field, value); } = string.Empty;

	private readonly ChangeTracker _changeTracker = new();

	public bool CanUndo => _changeTracker.CanUndo;
	public bool CanRedo => _changeTracker.CanRedo;

	public bool IsBusy { get; private set => Set(ref field, value); }

	public bool IsNew { get; set; }

	public TodoItemViewModel(
		ITodoItemService todoItemService,
		IUserService userService,
		ViewModelBaseParameters<TodoItemViewModel> parameters) : base(parameters)
	{
		_todoItemService = todoItemService;
		_userService = userService;

		NavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));
		SaveCommand = new AsyncCommand(SaveAsync, t => HandleError(t.Exception));
		DeleteCommand = new AsyncCommand(DeleteAsync, t => HandleError(t.Exception));
		UndoCommand = new RelayCommand(() => _changeTracker.Undo(), () => _changeTracker.CanUndo);
		RedoCommand = new RelayCommand(() => _changeTracker.Redo(), () => _changeTracker.CanRedo);

		ChangeVisitor = _changeTracker;
		_changeTracker.StateChanged += () =>
		{
			RaisePropertiesChanged(nameof(CanUndo), nameof(CanRedo));
			UndoCommand.RaiseCanExecuteChanged();
			RedoCommand.RaiseCanExecuteChanged();
		};

		_todoItemService.OnItemUpdated += Handle;
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
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			if (Id != obj.Id)
			{
				return;
			}

			Map.From(obj).To(this);
			_changeTracker.Clear();
			InitializeChecklistTracking();
		});

	public AsyncCommand NavigateBackCommand { get; }
	public AsyncCommand SaveCommand { get; }
	public RelayCommand UndoCommand { get; }
	public RelayCommand RedoCommand { get; }
	public async Task SaveAsync()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;

			ValidateAll();
			if (HasErrors)
			{
				return;
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
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync("Error saving..." + ex.Message);
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

			await _todoItemService.DeleteItemAsync(Id);
			await NavigationService.NavigateBack();
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync("Error deleting..." + ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}

	protected override IEnumerable<ValidationResult> Validate(string? propertyName)
	{
		return [];
	}

	protected override bool ShouldIgnorePropertyForChangesAndValidation(string? propertyName)
		=> base.ShouldIgnorePropertyForChangesAndValidation(propertyName)
			|| propertyName is nameof(IsBusy) or nameof(IsNew) or nameof(Id) or nameof(AvailableUsers) or nameof(AvailableTodoItemStatus);

	private bool _disposedValue;
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				_todoItemService.OnItemUpdated -= Handle;
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
