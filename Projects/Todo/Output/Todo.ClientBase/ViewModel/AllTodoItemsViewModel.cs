using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Todo.Client.Localisation;
using Todo.Client.MVVM;
using Todo.Client.Data.Services;
using Todo.Client.Data.Models;
using Todo.Contracts.Events;

namespace Todo.ClientBase.ViewModel;

public class AllTodoItemsViewModel : ViewModelBase, IAsyncInitializable, IDisposable
{
	private readonly IUserService _userService;
	private readonly ITodoItemService _todoItemService;

	public bool IsBusy { get; set => Set(ref field, value); }

	public string Filter
	{
		get;
		set => Set(ref field, value, () => TodoItemsView.Refresh());
	} = string.Empty;

	public ObservableCollectionView<TodoItemListItemViewModel> TodoItemsView { get; }
	public ObservableCollection<TodoItemListItemViewModel> TodoItems { get; } = [];

	public AllTodoItemsViewModel(
		ITodoItemService todoItemService,
		IUserService userService,
		ViewModelBaseParameters <TodoItemViewModel> parameters) : base(parameters)
	{
		_todoItemService = todoItemService;
		_userService = userService;

		TodoItemsView = new ObservableCollectionView<TodoItemListItemViewModel>(TodoItems, FilterTodoItem);

		_todoItemService.ItemCreated += Handle;
		_todoItemService.ItemUpdated += Handle;
		_todoItemService.ItemDeleted += Handle;

		NavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));
		AddTodoItemCommand = new AsyncCommand(AddTodoItemAsync, t => HandleError(t.Exception));
		EditTodoItemCommand = new AsyncCommand<TodoItemListItemViewModel>(EditTodoItemAsync, t => HandleError(t.Exception));
		RefreshTodoItemsCommand = new AsyncCommand(RefreshTodoItemsAsync, t => HandleError(t.Exception));
	}

	public async Task InitializeAsync()
	{
		await RefreshTodoItemsAsync();
		await _todoItemService.StartEventListener();
	}

	private bool FilterTodoItem(TodoItemListItemViewModel item)
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

	private async Task Handle(object? sender, TodoItemCreated obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			var item = Map.From(obj).To<TodoItemListItemViewModel>();
			TodoItems.Insert(0, item);
		});

	private async Task Handle(object? sender, TodoItemUpdated obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			if (TodoItems.FirstOrDefault(i => i.Id == obj.Id) is { } item)
			{
				Map.From(obj).To(item);
			}
		});

	private async Task Handle(object? sender, TodoItemDeleted obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			if (TodoItems.FirstOrDefault(i => i.Id == obj.Id) is { } item)
			{
				TodoItems.Remove(item);
			}
		});

	public AsyncCommand NavigateBackCommand { get; }
	public AsyncCommand<TodoItemListItemViewModel> EditTodoItemCommand { get; }
	public async Task EditTodoItemAsync(TodoItemListItemViewModel? model)
	{
		if (IsBusy || model is null)
		{
			return;
		}

		try
		{
			IsBusy = true;
			await NavigationService.NavigateForward<TodoItemViewModel>(new() {{ "Id", model.Id }});
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync("Error loading..." + ex.Message);
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
			await NavigationService.NavigateForward<TodoItemViewModel>();
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync("Error adding..." + ex.Message);
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
			var viewModels = Map.From<TodoItemListItemModel>(todoItems).To<TodoItemListItemViewModel>();

			foreach (var item in viewModels)
			{
				TodoItems.Add(item);
			}
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync("Error loading data..." + ex.Message);
		}
		finally
		{
			TodoItemsView.Enable();
			IsBusy = false;
		}
	}

	private bool _disposedValue;
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				_todoItemService.ItemCreated -= Handle;
				_todoItemService.ItemUpdated -= Handle;
				_todoItemService.ItemDeleted -= Handle;
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