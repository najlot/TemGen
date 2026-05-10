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
using Todo.Contracts.Notes;
using Todo.Client.Data.Notes;

namespace Todo.ClientBase.Notes;

public class NotesViewModel : ViewModelBase, IAsyncInitializable, IDisposable
{
	private readonly INoteService _noteService;
	private readonly IFavoriteService _favoriteService;
	private static readonly ItemType FavoriteTargetType = ItemType.Note;

	public bool IsBusy { get; set => Set(ref field, value, () => Filters.IsBusy = value); }
	public EntityFilterEditorViewModel Filters { get; }
	public ObservableCollection<NoteListItemViewModel> Notes { get; } = [];

	public NotesViewModel(
		INoteService noteService,
		IFavoriteService favoriteService,
		NoteFilterViewModel filters,
		ViewModelBaseParameters<NotesViewModel> parameters) : base(parameters)
	{
		_noteService = noteService;
		_favoriteService = favoriteService;
		Filters = filters;

		Filters.FilterChanged += async (s, e) => await LoadItemsAsync(e);

		NavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));
		AddNoteCommand = new AsyncCommand(AddNoteAsync, t => HandleError(t.Exception));
		EditNoteCommand = new AsyncCommand<NoteListItemViewModel>(EditNoteAsync, t => HandleError(t.Exception));

		_noteService.ItemCreated += Handle;
		_noteService.ItemUpdated += Handle;
		_noteService.ItemDeleted += Handle;
		_favoriteService.ItemCreated += HandleFavoriteCreated;
		_favoriteService.ItemDeleted += HandleFavoriteDeleted;
	}

	public AsyncCommand NavigateBackCommand { get; }
	public AsyncCommand<NoteListItemViewModel> EditNoteCommand { get; }
	public AsyncCommand AddNoteCommand { get; }

	public async Task InitializeAsync()
	{
		await Task.WhenAll(
			_favoriteService.StartEventListener(),
			_noteService.StartEventListener());
		await Filters.InitializeAsync();
	}

	private async Task LoadItemsAsync(EntityFilter? filter)
	{
		try
		{
			IsBusy = true;
			_lastFilter = filter;

			var itemsTask = filter is null || filter.Conditions.Count == 0
				? _noteService.GetItemsAsync()
				: _noteService.GetItemsAsync(filter);
			var favoritesTask = _favoriteService.GetItemsAsync(FavoriteTargetType);

			await Task.WhenAll(itemsTask, favoritesTask);

			var items = await itemsTask;
			var favoriteIds = (await favoritesTask)
				.Select(item => item.ItemId)
				.ToHashSet();

			var viewModels = Map.From<NoteListItemModel>(items).To<NoteListItemViewModel>();
			Notes.Clear();
			foreach (var item in viewModels)
			{
				item.IsFavorite = favoriteIds.Contains(item.Id);
				Notes.Add(item);
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

	private async Task Handle(object? sender, NoteCreated obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			if (_lastFilter is { Conditions.Count: > 0 })
			{
				return;
			}

			var item = Map.From(obj).To<NoteListItemViewModel>();
			item.IsFavorite = false;
			Notes.Insert(0, item);
		});

	private async Task Handle(object? sender, NoteUpdated obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			if (Notes.FirstOrDefault(i => i.Id == obj.Id) is { } item)
			{
				Map.From(obj).To(item);
			}
		});

	private async Task Handle(object? sender, NoteDeleted obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			if (Notes.FirstOrDefault(i => i.Id == obj.Id) is { } item)
			{
				Notes.Remove(item);
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
		if (Notes.FirstOrDefault(item => item.Id == id) is { } item)
		{
			item.IsFavorite = isFavorite;
		}
	}

	public async Task EditNoteAsync(NoteListItemViewModel? model)
	{
		if (IsBusy || model is null)
		{
			return;
		}

		try
		{
			IsBusy = true;
			await NavigationService.NavigateForward<NoteViewModel>(new() {{ "Id", model.Id }});
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

	public async Task AddNoteAsync()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;
			await NavigationService.NavigateForward<NoteViewModel>();
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
				_noteService.ItemCreated -= Handle;
				_noteService.ItemUpdated -= Handle;
				_noteService.ItemDeleted -= Handle;
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