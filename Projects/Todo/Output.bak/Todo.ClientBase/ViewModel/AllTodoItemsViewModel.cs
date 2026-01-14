using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Najlot.Map;
using Todo.Client.Localisation;
using Todo.Client.MVVM;
using Todo.Client.MVVM.ViewModel;
using Todo.Client.MVVM.Services;
using Todo.Client.Data.Services;
using Todo.Client.Data.Models;
using Todo.Contracts.Events;

namespace Todo.ClientBase.ViewModel;

public class AllTodoItemsViewModel : AbstractViewModel, IDisposable
{
	private readonly IUserService _userService;
	private readonly IErrorService _errorService;
	private readonly ITodoItemService _todoItemService;
	private readonly INavigationService _navigationService;
	private readonly IMessenger _messenger;
	private readonly IMap _map;

	private bool _isBusy;
	private string _filter;

	public bool IsBusy
	{
		get => _isBusy;
		set => Set(nameof(IsBusy), ref _isBusy, value);
	}

	public string Filter
	{
		get => _filter;
		set
		{
			Set(nameof(Filter), ref _filter, value);
			TodoItemsView.Refresh();
		}
	}

	public ObservableCollectionView<TodoItemListItemModel> TodoItemsView { get; }
	public ObservableCollection<TodoItemListItemModel> TodoItems { get; } = [];

	public AllTodoItemsViewModel(
		IErrorService errorService,
		IUserService userService,
		ITodoItemService todoItemService,
		INavigationService navigationService,
		IMessenger messenger,
		IMap map)
	{
		_errorService = errorService;
		_userService = userService;
		_todoItemService = todoItemService;
		_navigationService = navigationService;
		_messenger = messenger;
		_map = map;

		TodoItemsView = new ObservableCollectionView<TodoItemListItemModel>(TodoItems, FilterTodoItem);

		_messenger.Register<TodoItemCreated>(Handle);
		_messenger.Register<TodoItemUpdated>(Handle);
		_messenger.Register<TodoItemDeleted>(Handle);

		AddTodoItemCommand = new AsyncCommand(AddTodoItemAsync, DisplayError);
		EditTodoItemCommand = new AsyncCommand<TodoItemListItemModel>(EditTodoItemAsync, DisplayError);
		RefreshTodoItemsCommand = new AsyncCommand(RefreshTodoItemsAsync, DisplayError);
	}

	private bool FilterTodoItem(TodoItemListItemModel item)
	{
		if (string.IsNullOrEmpty(Filter))
		{
			return true;
		}

		var title = item.Title;
		if (!string.IsNullOrEmpty(title) && title.IndexOf(Filter, StringComparison.OrdinalIgnoreCase) != -1)
		{
			return true;
		}

		var content = item.Content;
		if (!string.IsNullOrEmpty(content) && content.IndexOf(Filter, StringComparison.OrdinalIgnoreCase) != -1)
		{
			return true;
		}

		return false;
	}

	private async Task DisplayError(Task task)
	{
		await _errorService.ShowAlertAsync(CommonLoc.Error, task.Exception);
	}

	private void Handle(TodoItemCreated obj)
	{
		TodoItems.Insert(0, new TodoItemListItemModel()
		{
			Id = obj.Id,
			Title = obj.Title,
			Content = obj.Content,
		});
	}

	private void Handle(TodoItemUpdated obj)
	{
		var oldItem = TodoItems.FirstOrDefault(i => i.Id == obj.Id);
		var index = -1;

		if (oldItem != null)
		{
			index = TodoItems.IndexOf(oldItem);

			if (index != -1)
			{
				TodoItems.RemoveAt(index);
			}
		}

		if (index == -1)
		{
			index = 0;
		}

		TodoItems.Insert(index, new TodoItemListItemModel()
		{
			Id = obj.Id,
			Title = obj.Title,
			Content = obj.Content,
		});
	}

	private void Handle(TodoItemDeleted obj)
	{
		var oldItem = TodoItems.FirstOrDefault(i => i.Id == obj.Id);

		if (oldItem != null)
		{
			TodoItems.Remove(oldItem);
		}
	}

	public AsyncCommand<TodoItemListItemModel> EditTodoItemCommand { get; }
	public async Task EditTodoItemAsync(TodoItemListItemModel model)
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;

			var item = await _todoItemService.GetItemAsync(model.Id);
			var viewModel = _map.From(item).To<TodoItemViewModel>();

			await _navigationService.NavigateForward(viewModel);
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error loading...", ex);
		}
		finally
		{
			IsBusy = false;
		}
	}

	public AsyncCommand AddTodoItemCommand { get; }
	public async Task AddTodoItemAsync()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;

			var item = _todoItemService.CreateTodoItem();
			var viewModel = _map.From(item).To<TodoItemViewModel>();
			viewModel.IsNew = true;

			await _navigationService.NavigateForward(viewModel);
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error adding...", ex);
		}
		finally
		{
			IsBusy = false;
		}
	}

	public AsyncCommand RefreshTodoItemsCommand { get; }
	public async Task RefreshTodoItemsAsync()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;
			TodoItemsView.Disable();
			Filter = "";

			TodoItems.Clear();

			var users = await _userService.GetItemsAsync();
			var todoItems = await _todoItemService.GetItemsAsync();

			foreach (var item in todoItems)
			{
				TodoItems.Add(item);
			}
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error loading data...", ex);
		}
		finally
		{
			TodoItemsView.Enable();
			IsBusy = false;
		}
	}

	public void Dispose() => _messenger.Unregister(this);
}