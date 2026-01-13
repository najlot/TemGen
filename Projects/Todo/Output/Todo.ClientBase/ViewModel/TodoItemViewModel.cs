using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Najlot.Map;
using Todo.Client.Data.Models;
using Todo.Client.Data.Services;
using Todo.Client.MVVM;
using Todo.Client.MVVM.Services;
using Todo.Client.MVVM.Validation;
using Todo.Client.MVVM.ViewModel;
using Todo.ClientBase.Validation;
using Todo.Contracts;
using Todo.Contracts.Events;

namespace Todo.ClientBase.ViewModel;

public partial class TodoItemViewModel : AbstractValidationViewModel, IDisposable
{
	private readonly IErrorService _errorService;
	private readonly INavigationService _navigationService;
	private readonly ITodoItemService _todoItemService;
	private readonly IMessenger _messenger;
	private readonly IMap _map;
	private IEnumerable<UserListItemModel> _availableUsers;

	public IEnumerable<UserListItemModel> AvailableUsers { get => _availableUsers; set => Set(nameof(AvailableUsers), ref _availableUsers, value); }
	public List<TodoItemStatus> AvailableTodoItemStatus { get; } = new(Enum.GetValues(typeof(TodoItemStatus)) as TodoItemStatus[]);

	private Guid _id;
	public Guid Id { get => _id; set => Set(ref _id, value); }

	private string _title = string.Empty;
	public string Title { get => _title; set => Set(ref _title, value); }

	private string _content = string.Empty;
	public string Content { get => _content; set => Set(ref _content, value); }

	private DateTime _createdAt;
	public DateTime CreatedAt { get => _createdAt; set => Set(ref _createdAt, value); }

	private string _createdBy = string.Empty;
	public string CreatedBy { get => _createdBy; set => Set(ref _createdBy, value); }

	private Guid _assignedTo;
	public Guid AssignedTo { get => _assignedTo; set => Set(ref _assignedTo, value); }

	private TodoItemStatus _status;
	public TodoItemStatus Status { get => _status; set => Set(ref _status, value); }

	private DateTime _changedAt;
	public DateTime ChangedAt { get => _changedAt; set => Set(ref _changedAt, value); }

	private string _changedBy = string.Empty;
	public string ChangedBy { get => _changedBy; set => Set(ref _changedBy, value); }

	private string _priority = string.Empty;
	public string Priority { get => _priority; set => Set(ref _priority, value); }

	private bool _isBusy;
	public bool IsBusy { get => _isBusy; private set => Set(ref _isBusy, value); }

	public bool IsNew { get; set; }

	public TodoItemViewModel(
		IErrorService errorService,
		INavigationService navigationService,
		ITodoItemService todoItemService,
		IMessenger messenger,
		IMap map)
	{
		_errorService = errorService;
		_navigationService = navigationService;
		_todoItemService = todoItemService;
		_messenger = messenger;
		_map = map;

		SaveCommand = new AsyncCommand(SaveAsync, DisplayError);
		DeleteCommand = new AsyncCommand(DeleteAsync, DisplayError);

		SetValidation(new TodoItemValidationList());

		_messenger.Register<TodoItemUpdated>(Handle);
	}

	private async Task DisplayError(Task task)
	{
		await _errorService.ShowAlertAsync("Error...", task.Exception);
	}

	public void Handle(TodoItemUpdated obj)
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

			var model = _map.From(this).To<TodoItemModel>();

			if (IsNew)
			{
				await _todoItemService.AddItemAsync(model);
				IsNew = false;
			}
			else
			{
				await _todoItemService.UpdateItemAsync(model);
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
				await _todoItemService.DeleteItemAsync(Id);
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
}