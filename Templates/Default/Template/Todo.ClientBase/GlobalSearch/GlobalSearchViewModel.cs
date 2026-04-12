using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Client.MVVM;
using <# Project.Namespace#>.Contracts.Shared;
using <# Project.Namespace#>.Client.Data.GlobalSearch;
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.NameLow == "user")).OrderByDescending(d => d.Name)
#>using <# Project.Namespace#>.ClientBase.<# definition.Name#>s;
<#end#>
namespace <# Project.Namespace#>.ClientBase.GlobalSearch;

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
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.NameLow == "user")).OrderBy(d => d.Name)
#>				case ItemType.<# definition.Name#>:
					await NavigationService.NavigateForward<<# definition.Name#>ViewModel>(new() { { "Id", item.Id } });
					break;
<#end#>				default:
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
<#cs SetOutputPathAndSkipOtherDefinitions()#>