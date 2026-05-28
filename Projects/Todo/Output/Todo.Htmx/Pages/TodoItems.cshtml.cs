using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Todo.Client.Data;
using Todo.Client.Data.Favorites;
using Todo.Client.Data.TodoItems;
using Todo.Client.Data.Users;
using Todo.Client.Data.Filters;
using Todo.Client.Data.Identity;
using Todo.Client.Localisation;
using Todo.Contracts.Filters;
using Todo.Contracts.Shared;
using Todo.Contracts.TodoItems;

namespace Todo.Htmx.Pages;

[Authorize]
public class TodoItemsModel : PageModel
{
	private static readonly JsonSerializerOptions FilterStateSerializerOptions = new()
	{
		PropertyNameCaseInsensitive = true,
	};

	private readonly ITodoItemService _todoItemService;
	private readonly IFavoriteService _favoriteService;
	private readonly IUserService _userService;
	private readonly IFilterService _filterService;
	private readonly ILogger<TodoItemsModel> _log;

	public List<TodoItemListItemModel> Items { get; private set; } = [];
	public IReadOnlyList<UserListItemModel> Users { get; private set; } = [];
	public IReadOnlyList<FilterFieldOption> AvailableFilterFields { get; private set; } = [];
	public IReadOnlyList<Filter> NamedFilters { get; private set; } = [];
	public bool HasDefaultFilter { get; private set; }
	public string? SelectedNamedFilterName { get; private set; }
	public IReadOnlyList<FilterCondition> FilterConditions { get; private set; } = [];
	public bool HasActiveFilter => FilterConditions.Count > 0;
	public HashSet<Guid> FavoriteItemIds { get; private set; } = [];

	public TodoItemsModel(
		ITodoItemService todoItemService,
		IUserService userService,
		IFilterService filterService,
		IFavoriteService favoriteService,
		ILogger<TodoItemsModel> log)
	{
		_todoItemService = todoItemService;
		_userService = userService;
		_filterService = filterService;
		_favoriteService = favoriteService;
		_log = log;
	}

	public async Task<IActionResult> OnGetAsync(
		[FromQuery] string? filterState,
		[FromQuery] string? namedFilter,
		[FromQuery] string? filterField,
		[FromQuery] FilterOperator? filterOperator,
		[FromQuery] string? filterValue)
	{
		try
		{
			var filters = await LoadFilterContextAsync().ConfigureAwait(false);

			var activeFilter = ResolveActiveFilter(filters, filterState, namedFilter, filterField, filterOperator, filterValue);
			FilterConditions = activeFilter.Conditions;
			var favoritesTask = _favoriteService.GetItemsAsync(ItemType.TodoItem);

			Items = HasActiveFilter
				? [.. await _todoItemService.GetItemsAsync(activeFilter).ConfigureAwait(false)]
				: [.. await _todoItemService.GetItemsAsync().ConfigureAwait(false)];

			var favoriteItems = await favoritesTask.ConfigureAwait(false);
			FavoriteItemIds = [.. favoriteItems.Select(item => item.ItemId)];
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error loading TodoItems.");
			TempData["ErrorTitle"] = TodoItemLoc.TodoItems;
			TempData["ErrorMessage"] = ex.Message;
		}

		if (Request.Headers.ContainsKey("HX-Request"))
		{
			return Partial("_TodoItemList", this);
		}

		return Page();
	}

	public async Task<IActionResult> OnPostSaveFilterAsync([FromForm] string? filterState, [FromForm] string? namedFilter, [FromForm] string? filterName)
	{
		try
		{
			var filters = await LoadFilterContextAsync().ConfigureAwait(false);
			var activeFilter = ResolveActiveFilter(filters, filterState, namedFilter, null, null, null);

			if (activeFilter.Conditions.Count == 0)
			{
				TempData["ErrorTitle"] = FilterLoc.Filters;
				TempData["ErrorMessage"] = FilterLoc.AddConditionBeforeSaving;
				return CreateFilterManagementRedirect(SerializeFilterState(activeFilter), namedFilter);
			}

			if (string.IsNullOrWhiteSpace(filterName))
			{
				TempData["ErrorTitle"] = FilterLoc.Filters;
				TempData["ErrorMessage"] = FilterLoc.EnterNameBeforeSaving;
				return CreateFilterManagementRedirect(SerializeFilterState(activeFilter), namedFilter);
			}

			var trimmedFilterName = filterName.Trim();
			var existingNamedFilter = filters.FirstOrDefault(savedFilter => !savedFilter.IsDefault && string.Equals(savedFilter.Name, trimmedFilterName, StringComparison.OrdinalIgnoreCase));
			var filterToSave = new Filter
			{
				Id = existingNamedFilter?.Id ?? Guid.NewGuid(),
				TargetType = ItemType.TodoItem,
				Name = trimmedFilterName,
				IsDefault = false,
				Conditions = CloneConditions(activeFilter.Conditions),
			};

			if (existingNamedFilter is null)
			{
				await _filterService.AddItemAsync(filterToSave).ConfigureAwait(false);
			}
			else
			{
				await _filterService.UpdateItemAsync(filterToSave).ConfigureAwait(false);
			}

			return CreateFilterManagementRedirect(SerializeFilterState(activeFilter), trimmedFilterName);
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error saving filter for TodoItems.");
			TempData["ErrorTitle"] = FilterLoc.Filters;
			TempData["ErrorMessage"] = ex.Message;
			return CreateFilterManagementRedirect(filterState, namedFilter);
		}
	}

	public async Task<IActionResult> OnPostDeleteFilterAsync([FromForm] string? filterState, [FromForm] string? namedFilter)
	{
		try
		{
			var filters = await LoadFilterContextAsync().ConfigureAwait(false);
			var activeFilter = ResolveActiveFilter(filters, filterState, namedFilter, null, null, null);

			if (!string.IsNullOrWhiteSpace(namedFilter)
				&& filters.FirstOrDefault(savedFilter => !savedFilter.IsDefault && string.Equals(savedFilter.Name, namedFilter, StringComparison.OrdinalIgnoreCase)) is { } selectedFilter)
			{
				await _filterService.DeleteItemAsync(selectedFilter.Id).ConfigureAwait(false);
			}

			return CreateFilterManagementRedirect(SerializeFilterState(activeFilter), null);
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error deleting filter for TodoItems.");
			TempData["ErrorTitle"] = FilterLoc.Filters;
			TempData["ErrorMessage"] = ex.Message;
			return CreateFilterManagementRedirect(filterState, namedFilter);
		}
	}

	public async Task<IActionResult> OnPostSetDefaultFilterAsync([FromForm] string? filterState, [FromForm] string? namedFilter)
	{
		try
		{
			var filters = await LoadFilterContextAsync().ConfigureAwait(false);
			var activeFilter = ResolveActiveFilter(filters, filterState, namedFilter, null, null, null);

			if (activeFilter.Conditions.Count == 0)
			{
				TempData["ErrorTitle"] = FilterLoc.Filters;
				TempData["ErrorMessage"] = FilterLoc.AddConditionBeforeSettingDefault;
				return CreateFilterManagementRedirect(SerializeFilterState(activeFilter), namedFilter);
			}

			var existingDefaultFilter = filters.FirstOrDefault(savedFilter => savedFilter.IsDefault);
			var defaultFilter = new Filter
			{
				Id = existingDefaultFilter?.Id ?? Guid.NewGuid(),
				TargetType = ItemType.TodoItem,
				Name = string.Empty,
				IsDefault = true,
				Conditions = CloneConditions(activeFilter.Conditions),
			};

			if (existingDefaultFilter is null)
			{
				await _filterService.AddItemAsync(defaultFilter).ConfigureAwait(false);
			}
			else
			{
				await _filterService.UpdateItemAsync(defaultFilter).ConfigureAwait(false);
			}

			return CreateFilterManagementRedirect(SerializeFilterState(activeFilter), namedFilter);
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error setting default filter for TodoItems.");
			TempData["ErrorTitle"] = FilterLoc.Filters;
			TempData["ErrorMessage"] = ex.Message;
			return CreateFilterManagementRedirect(filterState, namedFilter);
		}
	}

	public async Task<IActionResult> OnPostClearDefaultFilterAsync([FromForm] string? filterState, [FromForm] string? namedFilter)
	{
		try
		{
			var filters = await LoadFilterContextAsync().ConfigureAwait(false);
			var activeFilter = ResolveActiveFilter(filters, filterState, namedFilter, null, null, null);

			if (filters.FirstOrDefault(savedFilter => savedFilter.IsDefault) is { } defaultFilter)
			{
				await _filterService.DeleteItemAsync(defaultFilter.Id).ConfigureAwait(false);
			}

			return CreateFilterManagementRedirect(SerializeFilterState(activeFilter), namedFilter);
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error clearing default filter for TodoItems.");
			TempData["ErrorTitle"] = FilterLoc.Filters;
			TempData["ErrorMessage"] = ex.Message;
			return CreateFilterManagementRedirect(filterState, namedFilter);
		}
	}

	private async Task<Filter[]> LoadFilterContextAsync()
	{
				Users = await _userService.GetItemsAsync();
		AvailableFilterFields = BuildFilterFieldOptions(Users);
		var filters = await _filterService.GetItemsAsync(ItemType.TodoItem).ConfigureAwait(false);
		NamedFilters = [.. filters.Where(savedFilter => !savedFilter.IsDefault).OrderBy(savedFilter => savedFilter.Name, StringComparer.CurrentCultureIgnoreCase)];
		HasDefaultFilter = filters.Any(savedFilter => savedFilter.IsDefault);
		return filters;
	}

	private IActionResult CreateFilterManagementRedirect(string? filterState, string? namedFilter)
	{
		var redirectUrl = BuildListPageUrl(filterState, namedFilter);
		if (Request.Headers.ContainsKey("HX-Request"))
		{
			Response.Headers["HX-Redirect"] = redirectUrl;
			return Content(string.Empty);
		}

		return Redirect(redirectUrl);
	}

	private string BuildListPageUrl(string? filterState, string? namedFilter)
	{
		var queryParts = new List<string>();

		if (filterState is not null)
		{
			queryParts.Add($"filterState={Uri.EscapeDataString(filterState)}");
		}

		if (!string.IsNullOrWhiteSpace(namedFilter))
		{
			queryParts.Add($"namedFilter={Uri.EscapeDataString(namedFilter)}");
		}

		var path = Request.Path.ToString();
		return queryParts.Count == 0
			? path
			: $"{path}?{string.Join("&", queryParts)}";
	}

	private static string SerializeFilterState(EntityFilter filter)
	{
		return JsonSerializer.Serialize(filter.Conditions.Select(condition => new FilterConditionState
		{
			Field = condition.Field,
			Operator = condition.Operator.ToString(),
			Value = condition.Value,
		}).ToList());
	}

	private static List<FilterCondition> CloneConditions(IEnumerable<FilterCondition> conditions)
	{
		return [.. conditions.Select(condition => new FilterCondition
		{
			Field = condition.Field,
			Operator = condition.Operator,
			Value = condition.Value,
		})];
	}

	private EntityFilter ResolveActiveFilter(
		IReadOnlyList<Filter> filters,
		string? filterState,
		string? namedFilter,
		string? filterField,
		FilterOperator? filterOperator,
		string? filterValue)
	{
		var selectedNamedFilter = !string.IsNullOrWhiteSpace(namedFilter)
			? NamedFilters.FirstOrDefault(namedFilterOption => string.Equals(namedFilterOption.Name, namedFilter, StringComparison.OrdinalIgnoreCase))
			: null;

		SelectedNamedFilterName = selectedNamedFilter?.Name;

		if (TryDeserializeFilterState(filterState, out var queryFilter))
		{
			return NormalizeFilter(queryFilter);
		}

		if (selectedNamedFilter is not null)
		{
			return NormalizeFilter(ToEntityFilter(selectedNamedFilter));
		}

		if (TryCreateLegacyFilter(filterField, filterOperator, filterValue, out var legacyFilter))
		{
			return NormalizeFilter(legacyFilter);
		}

		if (filters.FirstOrDefault(savedFilter => savedFilter.IsDefault) is { } defaultFilter)
		{
			return NormalizeFilter(ToEntityFilter(defaultFilter));
		}

		return new EntityFilter();
	}

	private bool TryDeserializeFilterState(string? filterState, out EntityFilter filter)
	{
		if (filterState is null)
		{
			filter = new EntityFilter();
			return false;
		}

		if (string.IsNullOrWhiteSpace(filterState))
		{
			filter = new EntityFilter();
			return true;
		}

		try
		{
			var conditionStates = JsonSerializer.Deserialize<List<FilterConditionState>>(filterState, FilterStateSerializerOptions) ?? [];
			var conditions = new List<FilterCondition>();

			foreach (var conditionState in conditionStates)
			{
				if (conditionState is null || string.IsNullOrWhiteSpace(conditionState.Field))
				{
					continue;
				}

				if (!Enum.TryParse<FilterOperator>(conditionState.Operator, true, out var parsedOperator))
				{
					parsedOperator = FilterOperator.Equals;
				}

				conditions.Add(new FilterCondition
				{
					Field = conditionState.Field.Trim(),
					Operator = parsedOperator,
					Value = conditionState.Value,
				});
			}

			filter = new EntityFilter { Conditions = conditions };
		}
		catch (JsonException ex)
		{
			_log.LogWarning(ex, "Ignoring invalid filter state for TodoItems.");
			filter = new EntityFilter();
		}

		return true;
	}

	private bool TryCreateLegacyFilter(string? filterField, FilterOperator? filterOperator, string? filterValue, out EntityFilter filter)
	{
		if (string.IsNullOrWhiteSpace(filterField))
		{
			filter = new EntityFilter();
			return false;
		}

		filter = new EntityFilter
		{
			Conditions =
			[
				new FilterCondition
				{
					Field = filterField,
					Operator = filterOperator ?? FilterOperator.Equals,
					Value = filterValue,
				},
			],
		};

		return true;
	}

	private EntityFilter NormalizeFilter(EntityFilter filter)
	{
		var normalizedConditions = new List<FilterCondition>();

		foreach (var condition in filter.Conditions)
		{
			if (TryNormalizeCondition(condition, out var normalizedCondition))
			{
				normalizedConditions.Add(normalizedCondition);
			}
		}

		return new EntityFilter { Conditions = normalizedConditions };
	}

	private bool TryNormalizeCondition(FilterCondition? condition, out FilterCondition normalizedCondition)
	{
		normalizedCondition = new FilterCondition();

		if (condition is null)
		{
			return false;
		}

		if (AvailableFilterFields.FirstOrDefault(filterFieldOption => filterFieldOption.Key == condition.Field) is not { } filterFieldOption)
		{
			return false;
		}

		var operatorOption = filterFieldOption.Operators.FirstOrDefault(candidate => candidate.Value == condition.Operator)
			?? filterFieldOption.Operators.FirstOrDefault();

		if (operatorOption is null)
		{
			return false;
		}

		normalizedCondition.Field = filterFieldOption.Key;
		normalizedCondition.Operator = operatorOption.Value;

		if (!operatorOption.Value.RequiresValue())
		{
			normalizedCondition.Value = null;
			return true;
		}

		if (filterFieldOption.Values.Count > 0)
		{
			var selectedValue = filterFieldOption.Values.FirstOrDefault(valueOption => valueOption.Value == condition.Value)?.Value
				?? filterFieldOption.Values.FirstOrDefault()?.Value;

			if (string.IsNullOrWhiteSpace(selectedValue))
			{
				return false;
			}

			normalizedCondition.Value = selectedValue;
			return true;
		}

		var value = condition.Value?.Trim();
		if (string.IsNullOrWhiteSpace(value))
		{
			return false;
		}

		normalizedCondition.Value = value;
		return true;
	}

	private static EntityFilter ToEntityFilter(Filter filter)
	{
		return new EntityFilter
		{
			Conditions = [.. filter.Conditions.Select(condition => new FilterCondition
			{
				Field = condition.Field,
				Operator = condition.Operator,
				Value = condition.Value,
			})],
		};
	}

	private sealed class FilterConditionState
	{
		public string? Field { get; init; }
		public string? Operator { get; init; }
		public string? Value { get; init; }
	}

	private IReadOnlyList<FilterFieldOption> BuildFilterFieldOptions(IReadOnlyList<UserListItemModel> users)
	{
		return
		[
			new FilterFieldOption
			{
				Key = "Title",
				Label = TodoItemLoc.Title,
				Kind = FilterFieldKind.Text,
				Operators = FilterOperatorCatalog.CreateTextOptions(),
				Values = [],
			},
			new FilterFieldOption
			{
				Key = "Content",
				Label = TodoItemLoc.Content,
				Kind = FilterFieldKind.Text,
				Operators = FilterOperatorCatalog.CreateTextOptions(),
				Values = [],
			},
			new FilterFieldOption
			{
				Key = "AssignedToId",
				Label = TodoItemLoc.AssignedTo,
				Kind = FilterFieldKind.Option,
				Operators = FilterOperatorCatalog.CreateEqualityOptions(false),
				Values = [.. users.Select(item => new FilterValueOption
				{
					Value = item.Id.ToString(),
					Label = item.DisplayText,
				})],
			},
			new FilterFieldOption
			{
				Key = "Status",
				Label = TodoItemLoc.Status,
				Kind = FilterFieldKind.Option,
				Operators = FilterOperatorCatalog.CreateEqualityOptions(false),
				Values = [.. Enum.GetValues<TodoItemStatus>().Select(value => new FilterValueOption
				{
					Value = value.ToString(),
					Label = value == TodoItemStatus.None ? CommonLoc.NothingSelected : value.ToString(),
				})],
			},
			new FilterFieldOption
			{
				Key = "DueDate",
				Label = TodoItemLoc.DueDate,
				Kind = FilterFieldKind.DateTime,
				Operators = FilterOperatorCatalog.CreateComparableOptions(false),
				Values = [],
			},
			new FilterFieldOption
			{
				Key = "Priority",
				Label = TodoItemLoc.Priority,
				Kind = FilterFieldKind.Text,
				Operators = FilterOperatorCatalog.CreateTextOptions(),
				Values = [],
			},
		];
	}
}

