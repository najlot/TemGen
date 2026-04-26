using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data;
using <# Project.Namespace#>.Client.Data.Filters;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Client.MVVM;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.ClientBase.Filters;

public abstract class EntityFilterEditorViewModel : ViewModelBase
{
	private readonly IFilterService _filterService;
	private readonly ItemType _targetType;
	private List<Filter> _filters = [];

	public EntityFilterEditorViewModel(
		IFilterService filterService,
		ItemType targetType,
		IViewModelBaseParameters parameters) : base(parameters)
	{
		_filterService = filterService;
		_targetType = targetType;

		FilterConditions.CollectionChanged += HandleFilterConditionsChanged;

		SearchCommand = new AsyncCommand(SearchAsync, t => HandleError(t.Exception), () => !IsBusy);
		ClearFiltersCommand = new AsyncCommand(ClearFiltersAsync, t => HandleError(t.Exception), () => !IsBusy);
		AddFilterConditionCommand = new RelayCommand(AddFilterCondition, () => !IsBusy && !string.IsNullOrWhiteSpace(SelectedNewFilterFieldKey));
		RemoveFilterConditionCommand = new RelayCommand<FilterConditionViewModel>(RemoveFilterCondition, condition => !IsBusy && condition is not null);
		SaveCurrentFilterCommand = new AsyncCommand(SaveCurrentFilterAsync, t => HandleError(t.Exception), () => !IsBusy && FilterConditions.Count > 0 && !string.IsNullOrWhiteSpace(SaveFilterName));
		ApplyNamedFilterCommand = new AsyncCommand(ApplyNamedFilterAsync, t => HandleError(t.Exception), () => !IsBusy && !string.IsNullOrWhiteSpace(SelectedNamedFilterName));
		DeleteNamedFilterCommand = new AsyncCommand(DeleteNamedFilterAsync, t => HandleError(t.Exception), () => !IsBusy && !string.IsNullOrWhiteSpace(SelectedNamedFilterName));
		SetDefaultFilterCommand = new AsyncCommand(SetDefaultFilterAsync, t => HandleError(t.Exception), () => !IsBusy && FilterConditions.Count > 0);
		ClearDefaultFilterCommand = new AsyncCommand(ClearDefaultFilterAsync, t => HandleError(t.Exception), () => !IsBusy && HasDefaultFilter);
	}

	public event AsyncEventHandler<EntityFilter>? FilterChanged;

	protected abstract Task<IReadOnlyList<FilterFieldOption>> BuildFilterFieldOptionsAsync();

	public bool IsBusy
	{
		get;
		set => Set(ref field, value, () =>
		{
			SearchCommand.RaiseCanExecuteChanged();
			ClearFiltersCommand.RaiseCanExecuteChanged();
			AddFilterConditionCommand.RaiseCanExecuteChanged();
			RemoveFilterConditionCommand.RaiseCanExecuteChanged();
			SaveCurrentFilterCommand.RaiseCanExecuteChanged();
			ApplyNamedFilterCommand.RaiseCanExecuteChanged();
			DeleteNamedFilterCommand.RaiseCanExecuteChanged();
			SetDefaultFilterCommand.RaiseCanExecuteChanged();
			ClearDefaultFilterCommand.RaiseCanExecuteChanged();
		});
	}

	public IReadOnlyList<FilterFieldOption> AvailableFilterFields
	{
		get;
		private set => Set(ref field, value, () =>
		{
			if (string.IsNullOrWhiteSpace(SelectedNewFilterFieldKey) || value.All(filterField => filterField.Key != SelectedNewFilterFieldKey))
			{
				SelectedNewFilterFieldKey = value.FirstOrDefault()?.Key ?? string.Empty;
			}
			AddFilterConditionCommand.RaiseCanExecuteChanged();
		});
	} = [];

	public string SelectedNewFilterFieldKey
	{
		get;
		set => Set(ref field, value, AddFilterConditionCommand.RaiseCanExecuteChanged);
	} = string.Empty;

	public string SaveFilterName
	{
		get;
		set => Set(ref field, value, SaveCurrentFilterCommand.RaiseCanExecuteChanged);
	} = string.Empty;

	public string? SelectedNamedFilterName
	{
		get;
		set => Set(ref field, value, () =>
		{
			ApplyNamedFilterCommand.RaiseCanExecuteChanged();
			DeleteNamedFilterCommand.RaiseCanExecuteChanged();
		});
	}

	public ObservableCollection<FilterConditionViewModel> FilterConditions { get; } = [];
	public ObservableCollection<Filter> NamedFilters { get; } = [];
	public bool HasDefaultFilter => _filters.Any(filter => filter.IsDefault);

	public AsyncCommand SearchCommand { get; }
	public AsyncCommand ClearFiltersCommand { get; }
	public RelayCommand AddFilterConditionCommand { get; }
	public RelayCommand<FilterConditionViewModel> RemoveFilterConditionCommand { get; }
	public AsyncCommand SaveCurrentFilterCommand { get; }
	public AsyncCommand ApplyNamedFilterCommand { get; }
	public AsyncCommand DeleteNamedFilterCommand { get; }
	public AsyncCommand SetDefaultFilterCommand { get; }
	public AsyncCommand ClearDefaultFilterCommand { get; }

	public async Task InitializeAsync()
	{
		AvailableFilterFields = await BuildFilterFieldOptionsAsync();

		_filters = [.. await _filterService.GetItemsAsync(_targetType)];

		SyncNamedFilters();

		var defaultFilter = _filters.FirstOrDefault(filter => filter.IsDefault);
		var initialFilter = defaultFilter is not null
			? Map.From(defaultFilter).To<EntityFilter>()
			: new EntityFilter();

		LoadDraftFilter(initialFilter);
		await ApplyFilterAsync(BuildDraftFilter());
	}

	private async Task SearchAsync()
	{
		await ApplyFilterAsync(BuildDraftFilter());
	}

	private async Task ClearFiltersAsync()
	{
		LoadDraftFilter(new EntityFilter());
		await ApplyFilterAsync(new EntityFilter());
	}

	private void AddFilterCondition()
	{
		if (AvailableFilterFields.Count == 0) return;

		var fieldKey = string.IsNullOrWhiteSpace(SelectedNewFilterFieldKey)
			? AvailableFilterFields[0].Key
			: SelectedNewFilterFieldKey;

		FilterConditions.Add(new FilterConditionViewModel(AvailableFilterFields, new FilterCondition { Field = fieldKey }));
	}

	private void RemoveFilterCondition(FilterConditionViewModel? condition)
	{
		if (condition is not null)
		{
			FilterConditions.Remove(condition);
		}
	}

	private async Task SaveCurrentFilterAsync()
	{
		if (string.IsNullOrWhiteSpace(SaveFilterName))
		{
			await NotificationService.ShowErrorAsync(FilterLoc.EnterNameBeforeSaving);
			return;
		}

		var filter = BuildDraftFilter();
		if (filter.Conditions.Count == 0)
		{
			await NotificationService.ShowErrorAsync(FilterLoc.AddConditionBeforeSaving);
			return;
		}

		var filterName = SaveFilterName.Trim();
		var namedFilter = Map.From(filter).To<Filter>();

		namedFilter.TargetType = _targetType;
		namedFilter.Name = filterName;
		namedFilter.IsDefault = false;

		var existing = _filters.FirstOrDefault(f => !f.IsDefault && string.Equals(f.Name, filterName, StringComparison.OrdinalIgnoreCase));

		if (existing is null)
		{
			namedFilter.Id = Guid.NewGuid();
			await _filterService.AddItemAsync(namedFilter);
			_filters.Add(Map.From(namedFilter).To<Filter>());
		}
		else
		{
			namedFilter.Id = existing.Id;
			await _filterService.UpdateItemAsync(namedFilter);
			ReplaceFilter(namedFilter);
		}

		SyncNamedFilters();
		SelectedNamedFilterName = namedFilter.Name;
		SaveFilterName = string.Empty;
	}

	private async Task ApplyNamedFilterAsync()
	{
		if (GetSelectedNamedFilter() is not { } namedFilter) return;

		var filter = Map.From(namedFilter).To<EntityFilter>();
		LoadDraftFilter(filter);
		await ApplyFilterAsync(Map.From(filter).To<EntityFilter>());
	}

	private async Task DeleteNamedFilterAsync()
	{
		if (GetSelectedNamedFilter() is not { } namedFilter) return;

		await _filterService.DeleteItemAsync(namedFilter.Id);
		_filters.RemoveAll(filter => filter.Id == namedFilter.Id);

		SelectedNamedFilterName = null;
		SyncNamedFilters();
	}

	private async Task SetDefaultFilterAsync()
	{
		var filter = BuildDraftFilter();
		if (filter.Conditions.Count == 0)
		{
			await NotificationService.ShowErrorAsync(FilterLoc.AddConditionBeforeSettingDefault);
			return;
		}

		var defaultFilter = Map.From(filter).To<Filter>();
		defaultFilter.TargetType = _targetType;
		defaultFilter.Name = string.Empty;
		defaultFilter.IsDefault = true;

		if (_filters.FirstOrDefault(f => f.IsDefault) is { } existingDefault)
		{
			defaultFilter.Id = existingDefault.Id;
			await _filterService.UpdateItemAsync(defaultFilter);
			ReplaceFilter(defaultFilter);
		}
		else
		{
			await _filterService.AddItemAsync(defaultFilter);
			_filters.Add(Map.From(defaultFilter).To<Filter>());
		}

		SyncNamedFilters();
	}

	private async Task ClearDefaultFilterAsync()
	{
		if (_filters.FirstOrDefault(filter => filter.IsDefault) is not { } defaultFilter) return;

		await _filterService.DeleteItemAsync(defaultFilter.Id);
		_filters.RemoveAll(filter => filter.Id == defaultFilter.Id);
		SyncNamedFilters();
	}

	private async Task ApplyFilterAsync(EntityFilter filter)
	{
		var effectiveFilter = Map.From(filter).To<EntityFilter>();

		if (FilterChanged is { } filterChanged)
		{
			foreach (var handler in filterChanged.GetInvocationList().OfType<AsyncEventHandler<EntityFilter>>())
			{
				await handler.Invoke(this, effectiveFilter);
			}
		}
	}

	private EntityFilter BuildDraftFilter()
	{
		var conditions = Map
			.From<FilterConditionViewModel>(FilterConditions).To<FilterCondition>()
			.Where(c => AvailableFilterFields.Any(field => field.Key == c.Field))
			.Where(c => !c.Operator.RequiresValue() || !string.IsNullOrWhiteSpace(c.Value))
			.ToList();

		return new EntityFilter { Conditions = conditions };
	}

	private void LoadDraftFilter(EntityFilter filter)
	{
		var knownFieldKeys = AvailableFilterFields.Select(field => field.Key).ToHashSet(StringComparer.Ordinal);
		FilterConditions.Clear();

		foreach (var condition in filter.Conditions.Where(c => knownFieldKeys.Contains(c.Field)))
		{
			FilterConditions.Add(new FilterConditionViewModel(AvailableFilterFields, condition));
		}
	}

	private Filter? GetSelectedNamedFilter()
		=> NamedFilters.FirstOrDefault(namedFilter => string.Equals(namedFilter.Name, SelectedNamedFilterName, StringComparison.OrdinalIgnoreCase));

	private void SyncNamedFilters()
	{
		NamedFilters.Clear();
		foreach (var namedFilter in _filters.Where(filter => !filter.IsDefault).OrderBy(filter => filter.Name, StringComparer.CurrentCultureIgnoreCase))
		{
			NamedFilters.Add(Map.From(namedFilter).To<Filter>());
		}

		if (!string.IsNullOrWhiteSpace(SelectedNamedFilterName)
			&& NamedFilters.All(namedFilter => !string.Equals(namedFilter.Name, SelectedNamedFilterName, StringComparison.OrdinalIgnoreCase)))
		{
			SelectedNamedFilterName = null;
		}

		RaisePropertyChanged(nameof(HasDefaultFilter));
		ClearDefaultFilterCommand.RaiseCanExecuteChanged();
		ApplyNamedFilterCommand.RaiseCanExecuteChanged();
		DeleteNamedFilterCommand.RaiseCanExecuteChanged();
	}

	private void ReplaceFilter(Filter filter)
	{
		var index = _filters.FindIndex(existing => existing.Id == filter.Id);
		if (index >= 0)
		{
			_filters[index] = Map.From(filter).To<Filter>();
		}
		else
		{
			_filters.Add(Map.From(filter).To<Filter>());
		}
	}

	private void HandleFilterConditionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		SaveCurrentFilterCommand.RaiseCanExecuteChanged();
		SetDefaultFilterCommand.RaiseCanExecuteChanged();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>