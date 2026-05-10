using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Todo.Client.Data.Favorites;
using Todo.Client.Localisation;
using Todo.Client.MVVM;
using Todo.ClientBase.Filters;
using Todo.Contracts.Favorites;
using Todo.Contracts.Filters;
using Todo.Contracts.Shared;
using Todo.Contracts.TodoItems;
using Todo.Client.Data.TodoItems;

namespace Todo.ClientBase.TodoItems;

public class TodoItemsViewModel : ViewModelBase, IAsyncInitializable, IDisposable
{
	private readonly ITodoItemService _todoItemService;
	private readonly IFavoriteService _favoriteService;
	private static readonly ItemType FavoriteTargetType = ItemType.TodoItem;

	public bool IsBusy { get; set => Set(ref field, value, () => Filters.IsBusy = value); }
	public EntityFilterEditorViewModel Filters { get; }
	public ObservableCollection<TodoItemListItemViewModel> TodoItems { get; } = [];

	public TodoItemsViewModel(
		ITodoItemService todoItemService,
		IFavoriteService favoriteService,
		TodoItemFilterViewModel filters,
		ViewModelBaseParameters<TodoItemsViewModel> parameters) : base(parameters)
	{
		_todoItemService = todoItemService;
		_favoriteService = favoriteService;
		Filters = filters;

		Filters.FilterChanged += async (s, e) => await LoadItemsAsync(e);

		NavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));
		AddTodoItemCommand = new AsyncCommand(AddTodoItemAsync, t => HandleError(t.Exception));
		EditTodoItemCommand = new AsyncCommand<TodoItemListItemViewModel>(EditTodoItemAsync, t => HandleError(t.Exception));

		_todoItemService.ItemCreated += Handle;
		_todoItemService.ItemUpdated += Handle;
		_todoItemService.ItemDeleted += Handle;
		_favoriteService.ItemCreated += HandleFavoriteCreated;
		_favoriteService.ItemDeleted += HandleFavoriteDeleted;
	}

	public AsyncCommand NavigateBackCommand { get; }
	public AsyncCommand<TodoItemListItemViewModel> EditTodoItemCommand { get; }
	public AsyncCommand AddTodoItemCommand { get; }

	public async Task InitializeAsync()
	{
		await Task.WhenAll(
			_favoriteService.StartEventListener(),
			_todoItemService.StartEventListener());
		await Filters.InitializeAsync();
	}

	private async Task LoadItemsAsync(EntityFilter? filter)
	{
		try
		{
			IsBusy = true;
			_lastFilter = filter;

			var itemsTask = filter is null || filter.Conditions.Count == 0
				? _todoItemService.GetItemsAsync()
				: _todoItemService.GetItemsAsync(filter);
			var favoritesTask = _favoriteService.GetItemsAsync(FavoriteTargetType);

			await Task.WhenAll(itemsTask, favoritesTask);

			var items = await itemsTask;
			var favoriteIds = (await favoritesTask)
				.Select(item => item.ItemId)
				.ToHashSet();

			var viewModels = Map.From<TodoItemListItemModel>(items).To<TodoItemListItemViewModel>();
			TodoItems.Clear();
			foreach (var item in viewModels)
			{
				item.IsFavorite = favoriteIds.Contains(item.Id);
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
			item.IsFavorite = false;
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

	private async Task HandleFavoriteCreated(object? sender, FavoriteCreated obj)
	{
		if (obj.TargetType != FavoriteTargetType)
		{
			return;
		}

		await DispatcherHelper.InvokeOnUIThread(() => SetFavoriteState(obj.ItemId, true));
	}

	private async Task HandleFavoriteDeleted(object? sender, FavoriteDeleted obj)
	{
		if (obj.TargetType != FavoriteTargetType)
		{
			return;
		}

		await DispatcherHelper.InvokeOnUIThread(() => SetFavoriteState(obj.ItemId, false));
	}

	private void SetFavoriteState(Guid id, bool isFavorite)
	{
		if (TodoItems.FirstOrDefault(item => item.Id == id) is { } item)
		{
			item.IsFavorite = isFavorite;
		}
	}

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
				_favoriteService.ItemCreated -= HandleFavoriteCreated;
				_favoriteService.ItemDeleted -= HandleFavoriteDeleted;
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