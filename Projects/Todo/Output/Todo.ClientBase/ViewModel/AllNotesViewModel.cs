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

public class AllNotesViewModel : ViewModelBase, IAsyncInitializable, IDisposable
{
	private readonly INoteService _noteService;

	public bool IsBusy { get; set => Set(ref field, value); }

	public string Filter
	{
		get;
		set => Set(ref field, value, () => NotesView.Refresh());
	} = string.Empty;

	public ObservableCollectionView<NoteListItemViewModel> NotesView { get; }
	public ObservableCollection<NoteListItemViewModel> Notes { get; } = [];

	public AllNotesViewModel(
		INoteService noteService,
		ViewModelBaseParameters <NoteViewModel> parameters) : base(parameters)
	{
		_noteService = noteService;

		NotesView = new ObservableCollectionView<NoteListItemViewModel>(Notes, FilterNote);

		_noteService.OnItemCreated += Handle;
		_noteService.OnItemUpdated += Handle;
		_noteService.OnItemDeleted += Handle;

		NavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));
		AddNoteCommand = new AsyncCommand(AddNoteAsync, t => HandleError(t.Exception));
		EditNoteCommand = new AsyncCommand<NoteListItemViewModel>(EditNoteAsync, t => HandleError(t.Exception));
		RefreshNotesCommand = new AsyncCommand(RefreshNotesAsync, t => HandleError(t.Exception));
	}

	public async Task InitializeAsync()
	{
		await RefreshNotesAsync();
		await _noteService.StartEventListener();
	}

	private bool FilterNote(NoteListItemViewModel item)
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

	private async Task Handle(object? sender, NoteCreated obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
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

	public AsyncCommand NavigateBackCommand { get; }
	public AsyncCommand<NoteListItemViewModel> EditNoteCommand { get; }
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
			await NotificationService.ShowErrorAsync("Error loading..." + ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}

	public AsyncCommand AddNoteCommand { get; }
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
			await NotificationService.ShowErrorAsync("Error adding..." + ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}

	public AsyncCommand RefreshNotesCommand { get; }
	public async Task RefreshNotesAsync()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;
			NotesView.Disable();
			Filter = "";

			Notes.Clear();

			var notes = await _noteService.GetItemsAsync();
			var viewModels = Map.From<NoteListItemModel>(notes).To<NoteListItemViewModel>();

			foreach (var item in viewModels)
			{
				Notes.Add(item);
			}
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync("Error loading data..." + ex.Message);
		}
		finally
		{
			NotesView.Enable();
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
				_noteService.OnItemCreated -= Handle;
				_noteService.OnItemUpdated -= Handle;
				_noteService.OnItemDeleted -= Handle;
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