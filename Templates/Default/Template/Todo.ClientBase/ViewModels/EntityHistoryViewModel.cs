using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Services;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Client.MVVM;
using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.ClientBase.ViewModels;

public sealed class EntityHistoryViewModel : ViewModelBase, IParameterizable, IAsyncInitializable
{
	private readonly IHistoryService _historyService;

	public EntityHistoryViewModel(
		IHistoryService historyService,
		ViewModelBaseParameters<EntityHistoryViewModel> parameters) : base(parameters)
	{
		_historyService = historyService;
		NavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));
	}

	public Guid EntityId { get; private set => Set(ref field, value); }

	public string EntityName { get; private set => Set(ref field, value, () => RaisePropertyChanged(nameof(HasEntityName))); } = string.Empty;

	public bool HasEntityName => !string.IsNullOrWhiteSpace(EntityName);

	public HistoryListItemViewModel[] Items
	{
		get;
		private set => Set(ref field, value, () => RaisePropertyChanged(nameof(ShowNoHistory)));
	} = [];

	public bool IsLoading
	{
		get;
		private set => Set(ref field, value, () => RaisePropertyChanged(nameof(ShowNoHistory)));
	}

	public bool ShowNoHistory => !IsLoading && Items.Length == 0;

	public AsyncCommand NavigateBackCommand { get; }

	public void SetParameters(IReadOnlyDictionary<string, object> parameters)
	{
		if (parameters.TryGetValue("EntityId", out var entityIdObj) && entityIdObj is Guid entityId)
		{
			EntityId = entityId;
		}

		if (parameters.TryGetValue("EntityName", out var entityNameObj) && entityNameObj is string entityName)
		{
			EntityName = entityName;
		}
	}

	public async Task InitializeAsync()
	{
		if (EntityId == Guid.Empty || IsLoading)
		{
			return;
		}

		try
		{
			IsLoading = true;
			var entries = await _historyService.GetItemsAsync(EntityId);
			Items = entries.Select(HistoryListItemViewModel.Create).ToArray();
		}
		catch (Exception ex)
		{
			Items = [];
			await NotificationService.ShowErrorAsync($"{ErrorLoc.ErrorLoadingData} {ex.Message}");
		}
		finally
		{
			IsLoading = false;
		}
	}
}

public sealed class HistoryListItemViewModel
{
	public string Username { get; init; } = "-";
	public string TimeStampText { get; init; } = string.Empty;
	public HistoryChange[] Changes { get; init; } = [];
	public int ChangeCount => Changes.Length;

	public static HistoryListItemViewModel Create(HistoryEntry entry)
	{
		return new HistoryListItemViewModel
		{
			Username = string.IsNullOrWhiteSpace(entry.Username) ? "-" : entry.Username,
			TimeStampText = entry.TimeStamp.ToLocalTime().ToString("g"),
			Changes = entry.Changes
		};
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>