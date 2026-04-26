using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Todo.Client.Localisation;
using Todo.Client.MVVM;
using Todo.ClientBase.Filters;
using Todo.Contracts.Filters;
using Todo.Contracts.TodoItems;
using Todo.Client.Data.TodoItems;

namespace Todo.ClientBase.TodoItems;

public class TodoItemsViewModel : ViewModelBase, IAsyncInitializable, IDisposable
{
	private readonly ITodoItemService _todoItemService;

	public bool IsBusy { get; set => Set(ref field, value, () => Filters.IsBusy = value); }
	public EntityFilterEditorViewModel Filters { get; }
	public ObservableCollection<TodoItemListItemViewModel> TodoItems { get; } = [];

	public TodoItemsViewModel(
		ITodoItemService todoItemService,
		TodoItemFilterViewModel filters,
		ViewModelBaseParameters<TodoItemsViewModel> parameters) : base(parameters)
	{
		_todoItemService = todoItemService;
		Filters = filters;

		Filters.FilterChanged += async (s, e) => await LoadItemsAsync(e);

		NavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));
		AddTodoItemCommand = new AsyncCommand(AddTodoItemAsync, t => HandleError(t.Exception));
		EditTodoItemCommand = new AsyncCommand<TodoItemListItemViewModel>(EditTodoItemAsync, t => HandleError(t.Exception));

		_todoItemService.ItemCreated += Handle;
		_todoItemService.ItemUpdated += Handle;
		_todoItemService.ItemDeleted += Handle;
	}

	public AsyncCommand NavigateBackCommand { get; }
	public AsyncCommand<TodoItemListItemViewModel> EditTodoItemCommand { get; }
	public AsyncCommand AddTodoItemCommand { get; }

	public async Task InitializeAsync()
	{
		await Filters.InitializeAsync();
		await _todoItemService.StartEventListener();
	}

	private async Task LoadItemsAsync(EntityFilter? filter)
	{
		try
		{
			IsBusy = true;
			_lastFilter = filter;

			var items = filter is null || filter.Conditions.Count == 0
				? await _todoItemService.GetItemsAsync()
				: await _todoItemService.GetItemsAsync(filter);

			var viewModels = Map.From<TodoItemListItemModel>(items).To<TodoItemListItemViewModel>();
			TodoItems.Clear();
			foreach (var item in viewModels)
			{
				TodoItems.Add(item);
			}
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync($"{ErrorLoc.ErrorLoadingData} {ex.Message}");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private EntityFilter? _lastFilter;

	private async Task Handle(object? sender, TodoItemCreated obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			if (_lastFilter is { Conditions.Count: > 0 })
			{
				return;
			}

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
			await NotificationService.ShowErrorAsync($"{ErrorLoc.ErrorLoading} {ex.Message}");
		}
		finally
		{
			IsBusy = false;
		}
	}

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
			await NotificationService.ShowErrorAsync($"{ErrorLoc.ErrorAdding} {ex.Message}");
		}
		finally
		{
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