using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Client.Data.Services;
using Todo.Client.Localisation;
using Todo.Client.MVVM;
using Todo.Contracts;

namespace Todo.ClientBase.ViewModel;

public sealed class GlobalSearchViewModel : ViewModelBase, IAsyncInitializable
{
	private readonly IGlobalSearchService _globalSearchService;
	private bool _isBusy;
	private string _activeQuery = string.Empty;

	public GlobalSearchViewModel(IGlobalSearchService globalSearchService, ViewModelBaseParameters<GlobalSearchViewModel> parameters) : base(parameters)
	{
		_globalSearchService = globalSearchService;

		NavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));
		SearchCommand = new AsyncCommand(SearchAsync, t => HandleError(t.Exception));
		OpenResultCommand = new AsyncCommand<GlobalSearchItemViewModel>(OpenResultAsync, t => HandleError(t.Exception));
	}

	public bool IsBusy { get => _isBusy; set => Set(ref _isBusy, value, RaiseSearchStateProperties); }
	public string QueryText { get; set => Set(ref field, value); } = string.Empty;
	public string ActiveQuery { get => _activeQuery; set => Set(ref _activeQuery, value, RaiseSearchStateProperties); }
	public ObservableCollection<GlobalSearchItemViewModel> Items { get; } = [];
	public bool ShowSearchPrompt => !IsBusy && string.IsNullOrWhiteSpace(ActiveQuery);
	public bool ShowNoResults => !IsBusy && !string.IsNullOrWhiteSpace(ActiveQuery) && Items.Count == 0;

	public AsyncCommand NavigateBackCommand { get; }
	public AsyncCommand SearchCommand { get; }
	public AsyncCommand<GlobalSearchItemViewModel> OpenResultCommand { get; }

	public Task InitializeAsync()
	{
		return Task.CompletedTask;
	}

	public async Task SearchAsync()
	{
		if (IsBusy)
		{
			return;
		}

		var searchText = QueryText.Trim();
		ActiveQuery = searchText;
		Items.Clear();
		RaiseSearchStateProperties();

		if (string.IsNullOrWhiteSpace(searchText))
		{
			return;
		}

		try
		{
			IsBusy = true;
			var items = await _globalSearchService.SearchAsync(searchText).ConfigureAwait(false);
			var viewModels = Map.From<GlobalSearchItemModel>(items).ToArray<GlobalSearchItemViewModel>();

			foreach (var item in viewModels)
			{
				Items.Add(item);
			}

			RaiseSearchStateProperties();
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync($"{GlobalSearchLoc.GlobalSearch} {ex.Message}");
		}
		finally
		{
			IsBusy = false;
		}
	}

	public async Task OpenResultAsync(GlobalSearchItemViewModel? item)
	{
		if (IsBusy || item is null)
		{
			return;
		}

		try
		{
			IsBusy = true;

			switch (item.Type)
			{
				case ItemType.Note:
					await NavigationService.NavigateForward<NoteViewModel>(new() { { "Id", item.Id } }).ConfigureAwait(false);
					break;
				case ItemType.TodoItem:
					await NavigationService.NavigateForward<TodoItemViewModel>(new() { { "Id", item.Id } }).ConfigureAwait(false);
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

	private void RaiseSearchStateProperties()
	{
		RaisePropertiesChanged(nameof(ShowSearchPrompt), nameof(ShowNoResults));
	}
}
