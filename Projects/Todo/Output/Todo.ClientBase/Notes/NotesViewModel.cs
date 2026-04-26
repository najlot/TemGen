using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Todo.Client.Localisation;
using Todo.Client.MVVM;
using Todo.ClientBase.Filters;
using Todo.Contracts.Filters;
using Todo.Contracts.Notes;
using Todo.Client.Data.Notes;

namespace Todo.ClientBase.Notes;

public class NotesViewModel : ViewModelBase, IAsyncInitializable, IDisposable
{
	private readonly INoteService _noteService;

	public bool IsBusy { get; set => Set(ref field, value, () => Filters.IsBusy = value); }
	public EntityFilterEditorViewModel Filters { get; }
	public ObservableCollection<NoteListItemViewModel> Notes { get; } = [];

	public NotesViewModel(
		INoteService noteService,
		NoteFilterViewModel filters,
		ViewModelBaseParameters<NotesViewModel> parameters) : base(parameters)
	{
		_noteService = noteService;
		Filters = filters;

		Filters.FilterChanged += async (s, e) => await LoadItemsAsync(e);

		NavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));
		AddNoteCommand = new AsyncCommand(AddNoteAsync, t => HandleError(t.Exception));
		EditNoteCommand = new AsyncCommand<NoteListItemViewModel>(EditNoteAsync, t => HandleError(t.Exception));

		_noteService.ItemCreated += Handle;
		_noteService.ItemUpdated += Handle;
		_noteService.ItemDeleted += Handle;
	}

	public AsyncCommand NavigateBackCommand { get; }
	public AsyncCommand<NoteListItemViewModel> EditNoteCommand { get; }
	public AsyncCommand AddNoteCommand { get; }

	public async Task InitializeAsync()
	{
		await Filters.InitializeAsync();
		await _noteService.StartEventListener();
	}

	private async Task LoadItemsAsync(EntityFilter? filter)
	{
		try
		{
			IsBusy = true;
			_lastFilter = filter;

			var items = filter is null || filter.Conditions.Count == 0
				? await _noteService.GetItemsAsync()
				: await _noteService.GetItemsAsync(filter);

			var viewModels = Map.From<NoteListItemModel>(items).To<NoteListItemViewModel>();
			Notes.Clear();
			foreach (var item in viewModels)
			{
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