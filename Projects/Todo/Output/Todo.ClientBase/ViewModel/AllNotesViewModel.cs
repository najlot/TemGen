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

public class AllNotesViewModel : AbstractViewModel, IDisposable
{
	private readonly IErrorService _errorService;
	private readonly INoteService _noteService;
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
			NotesView.Refresh();
		}
	}

	public ObservableCollectionView<NoteListItemModel> NotesView { get; }
	public ObservableCollection<NoteListItemModel> Notes { get; } = [];

	public AllNotesViewModel(
		IErrorService errorService,
		INoteService noteService,
		INavigationService navigationService,
		IMessenger messenger,
		IMap map)
	{
		_errorService = errorService;
		_noteService = noteService;
		_navigationService = navigationService;
		_messenger = messenger;
		_map = map;

		NotesView = new ObservableCollectionView<NoteListItemModel>(Notes, FilterNote);

		_messenger.Register<NoteCreated>(Handle);
		_messenger.Register<NoteUpdated>(Handle);
		_messenger.Register<NoteDeleted>(Handle);

		AddNoteCommand = new AsyncCommand(AddNoteAsync, DisplayError);
		EditNoteCommand = new AsyncCommand<NoteListItemModel>(EditNoteAsync, DisplayError);
		RefreshNotesCommand = new AsyncCommand(RefreshNotesAsync, DisplayError);
	}

	private bool FilterNote(NoteListItemModel item)
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

	private void Handle(NoteCreated obj)
	{
		Notes.Insert(0, new NoteListItemModel()
		{
			Id = obj.Id,
			Title = obj.Title,
			Content = obj.Content,
		});
	}

	private void Handle(NoteUpdated obj)
	{
		var oldItem = Notes.FirstOrDefault(i => i.Id == obj.Id);
		var index = -1;

		if (oldItem != null)
		{
			index = Notes.IndexOf(oldItem);

			if (index != -1)
			{
				Notes.RemoveAt(index);
			}
		}

		if (index == -1)
		{
			index = 0;
		}

		Notes.Insert(index, new NoteListItemModel()
		{
			Id = obj.Id,
			Title = obj.Title,
			Content = obj.Content,
		});
	}

	private void Handle(NoteDeleted obj)
	{
		var oldItem = Notes.FirstOrDefault(i => i.Id == obj.Id);

		if (oldItem != null)
		{
			Notes.Remove(oldItem);
		}
	}

	public AsyncCommand<NoteListItemModel> EditNoteCommand { get; }
	public async Task EditNoteAsync(NoteListItemModel model)
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;

			var item = await _noteService.GetItemAsync(model.Id);
			var viewModel = _map.From(item).To<NoteViewModel>();

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

			var item = _noteService.CreateNote();
			var viewModel = _map.From(item).To<NoteViewModel>();
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

			foreach (var item in notes)
			{
				Notes.Add(item);
			}
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error loading data...", ex);
		}
		finally
		{
			NotesView.Enable();
			IsBusy = false;
		}
	}

	public void Dispose() => _messenger.Unregister(this);
}