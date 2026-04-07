using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Client.Data.Services;
using Todo.Client.Localisation;
using Todo.Client.MVVM;
using Todo.Contracts;
using Todo.Contracts.Events;

namespace Todo.ClientBase.ViewModels;

public sealed class TrashViewModel : ViewModelBase, IAsyncInitializable, IDisposable
{
	private readonly ITrashService _trashService;

	public TrashViewModel(ITrashService trashService, ViewModelBaseParameters<TrashViewModel> parameters) : base(parameters)
	{
		_trashService = trashService;
		TrashItemsView = new ObservableCollectionView<TrashItemViewModel>(TrashItems, FilterTrashItem, item => item.DeletedAt ?? DateTime.MinValue, true);

		_trashService.ItemCreated += Handle;
		_trashService.ItemUpdated += Handle;
		_trashService.ItemDeleted += Handle;

		NavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));
		OpenTrashItemCommand = new AsyncCommand<TrashItemViewModel>(OpenTrashItemAsync, t => HandleError(t.Exception));
		RestoreTrashItemCommand = new AsyncCommand<TrashItemViewModel>(RestoreTrashItemAsync, t => HandleError(t.Exception));
		DeleteTrashItemCommand = new AsyncCommand<TrashItemViewModel>(DeleteTrashItemAsync, t => HandleError(t.Exception));
		DeleteAllTrashCommand = new AsyncCommand(DeleteAllTrashAsync, t => HandleError(t.Exception));
	}

	public bool IsBusy { get; set => Set(ref field, value); }

	public string Filter
	{
		get;
		set => Set(ref field, value, () => TrashItemsView.Refresh());
	} = string.Empty;

	public ObservableCollection<TrashItemViewModel> TrashItems { get; } = [];
	public ObservableCollectionView<TrashItemViewModel> TrashItemsView { get; }
	public DeleteConfirmationDialogViewModel DeleteDialogViewModel { get; } = new();

	public AsyncCommand NavigateBackCommand { get; }
	public AsyncCommand<TrashItemViewModel> OpenTrashItemCommand { get; }
	public AsyncCommand<TrashItemViewModel> RestoreTrashItemCommand { get; }
	public AsyncCommand<TrashItemViewModel> DeleteTrashItemCommand { get; }
	public AsyncCommand DeleteAllTrashCommand { get; }

	public async Task InitializeAsync()
	{
		await RefreshTrashAsync();
		await _trashService.StartEventListener();
	}

	private async Task Handle(object? sender, TrashItemCreated obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			TrashItems.Insert(0, Map.From(obj).To<TrashItemViewModel>());
		});

	private async Task Handle(object? sender, TrashItemUpdated obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			if (TrashItems.FirstOrDefault(item => item.Id == obj.Id && item.Type == obj.Type) is { } item)
			{
				Map.From(obj).To(item);
			}
		});

	private async Task Handle(object? sender, TrashItemDeleted obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			if (TrashItems.FirstOrDefault(item => item.Id == obj.Id && item.Type == obj.Type) is { } item)
			{
				TrashItems.Remove(item);
			}
		});

	private bool FilterTrashItem(TrashItemViewModel item)
	{
		if (string.IsNullOrWhiteSpace(Filter))
		{
			return true;
		}

		return item.Title.Contains(Filter, StringComparison.OrdinalIgnoreCase)
			|| item.Content.Contains(Filter, StringComparison.OrdinalIgnoreCase)
			|| item.TypeDisplay.Contains(Filter, StringComparison.OrdinalIgnoreCase)
			|| item.DeletedAtDisplay.Contains(Filter, StringComparison.OrdinalIgnoreCase);
	}

	public async Task RefreshTrashAsync()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;
			TrashItemsView.Disable();
			Filter = string.Empty;
			TrashItems.Clear();

			var items = await _trashService.GetItemsAsync();
			var viewModels = Map.From<TrashItemModel>(items).ToArray<TrashItemViewModel>();

			foreach (var item in viewModels)
			{
				TrashItems.Add(item);
			}
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync($"{ErrorLoc.ErrorLoadingData} {ex.Message}");
		}
		finally
		{
			TrashItemsView.Enable();
			IsBusy = false;
		}
	}

	public async Task OpenTrashItemAsync(TrashItemViewModel? item)
	{
		if (IsBusy || item is null || DeleteDialogViewModel.IsVisible)
		{
			return;
		}

		try
		{
			IsBusy = true;

			switch (item.Type)
			{
				case ItemType.Note:
					await NavigationService.NavigateForward<NoteViewModel>(new() { { "Id", item.Id } });
					break;
				case ItemType.TodoItem:
					await NavigationService.NavigateForward<TodoItemViewModel>(new() { { "Id", item.Id } });
					break;
				default:
					break;
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

	public async Task RestoreTrashItemAsync(TrashItemViewModel? item)
	{
		if (IsBusy || item is null)
		{
			return;
		}

		try
		{
			IsBusy = true;
			await _trashService.RestoreItemAsync(item.Type, item.Id);
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync($"{TrashLoc.Restore} {ex.Message}");
		}
		finally
		{
			IsBusy = false;
		}
	}

	public async Task DeleteTrashItemAsync(TrashItemViewModel? item)
	{
		if (IsBusy || item is null || DeleteDialogViewModel.IsVisible)
		{
			return;
		}

		DeleteDialogViewModel.Title = TrashLoc.PermanentDeleteConfirmationTitle;
		DeleteDialogViewModel.Description = TrashLoc.PermanentDeleteConfirmationDescription;

		if (await DeleteDialogViewModel.ShouldDelete() == DeleteDialogResult.Cancel)
		{
			return;
		}

		try
		{
			IsBusy = true;
			await _trashService.DeleteItemAsync(item.Type, item.Id);
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync($"{CommonLoc.Delete} {ex.Message}");
		}
		finally
		{
			IsBusy = false;
		}
	}

	public async Task DeleteAllTrashAsync()
	{
		if (IsBusy || DeleteDialogViewModel.IsVisible || TrashItems.Count == 0)
		{
			return;
		}

		DeleteDialogViewModel.Title = TrashLoc.PermanentDeleteConfirmationTitle;
		DeleteDialogViewModel.Description = TrashLoc.PermanentDeleteAllConfirmationDescription;

		if (await DeleteDialogViewModel.ShouldDelete() == DeleteDialogResult.Cancel)
		{
			return;
		}

		try
		{
			IsBusy = true;
			await _trashService.DeleteAllItemsAsync();
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync($"{TrashLoc.DeleteAll} {ex.Message}");
		}
		finally
		{
			IsBusy = false;
		}
	}

	public void Dispose()
	{
		_trashService.ItemCreated -= Handle;
		_trashService.ItemUpdated -= Handle;
		_trashService.ItemDeleted -= Handle;
		GC.SuppressFinalize(this);
	}
}
