using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Todo.Client.Localisation;
using Todo.Client.MVVM;
using Todo.Contracts.Shared;
using Todo.Client.Data.GlobalSearch;
using Todo.ClientBase.TodoItems;
using Todo.ClientBase.Notes;

namespace Todo.ClientBase.GlobalSearch;

public sealed class GlobalSearchViewModel : ViewModelBase
{
	private readonly IGlobalSearchService _globalSearchService;

	public GlobalSearchViewModel(IGlobalSearchService globalSearchService, ViewModelBaseParameters<GlobalSearchViewModel> parameters) : base(parameters)
	{
		_globalSearchService = globalSearchService;

		NavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));
		SearchCommand = new AsyncCommand(SearchAsync, t => HandleError(t.Exception), () => !IsBusy && QueryText.Length > 2);
		OpenResultCommand = new AsyncCommand<GlobalSearchItemViewModel>(OpenResultAsync, t => HandleError(t.Exception));
	}

	public bool IsBusy
	{
		get;
		set => Set(ref field, value, () =>
		{
			SearchCommand.RaiseCanExecuteChanged();
			RaiseSearchStateProperties();
		});
	}
	
	public string QueryText { get; set => Set(ref field, value, () => SearchCommand.RaiseCanExecuteChanged()); } = string.Empty;
	public string ActiveQuery { get; set => Set(ref field, value, RaiseSearchStateProperties); } = string.Empty;
	public ObservableCollection<GlobalSearchItemViewModel> Items { get; } = [];
	public bool ShowSearchPrompt => !IsBusy && string.IsNullOrWhiteSpace(ActiveQuery);
	public bool ShowNoResults => !IsBusy && !string.IsNullOrWhiteSpace(ActiveQuery) && Items.Count == 0;

	public AsyncCommand NavigateBackCommand { get; }
	public AsyncCommand SearchCommand { get; }
	public AsyncCommand<GlobalSearchItemViewModel> OpenResultCommand { get; }

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
			var items = await _globalSearchService.SearchAsync(searchText);
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

	private void RaiseSearchStateProperties()
	{
		RaisePropertiesChanged(nameof(ShowSearchPrompt), nameof(ShowNoResults));
	}
}
