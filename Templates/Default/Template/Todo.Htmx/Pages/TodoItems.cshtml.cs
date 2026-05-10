using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using <# Project.Namespace#>.Client.Data;
using <# Project.Namespace#>.Client.Data.Favorites;
using <# Project.Namespace#>.Client.Data.<# Definition.Name#>s;
<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))
#>using <# Project.Namespace#>.Client.Data.<# definition.Name#>s;
<#end#>using <# Project.Namespace#>.Client.Data.Filters;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Contracts.Shared;
using <# Project.Namespace#>.Contracts.<# Definition.Name#>s;

namespace <# Project.Namespace#>.Htmx.Pages;

[Authorize]
public class <# Definition.Name#>sModel : PageModel
{
	private static readonly JsonSerializerOptions FilterStateSerializerOptions = new()
	{
		PropertyNameCaseInsensitive = true,
	};

	private readonly I<# Definition.Name#>Service _<# Definition.NameLow#>Service;
	private readonly IFavoriteService _favoriteService;
<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))
#>	private readonly I<# definition.Name#>Service _<# definition.NameLow#>Service;
<#end#>	private readonly IFilterService _filterService;
	private readonly ILogger<<# Definition.Name#>sModel> _log;

	public List<<# Definition.Name#>ListItemModel> Items { get; private set; } = [];
<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))
#>	public IReadOnlyList<<# definition.Name#>ListItemModel> <# definition.Name#>s { get; private set; } = [];
<#end#>	public IReadOnlyList<FilterFieldOption> AvailableFilterFields { get; private set; } = [];
	public IReadOnlyList<Filter> NamedFilters { get; private set; } = [];
	public bool HasDefaultFilter { get; private set; }
	public string? SelectedNamedFilterName { get; private set; }
	public IReadOnlyList<FilterCondition> FilterConditions { get; private set; } = [];
	public bool HasActiveFilter => FilterConditions.Count > 0;
	public HashSet<Guid> FavoriteItemIds { get; private set; } = [];

	public <# Definition.Name#>sModel(
		I<# Definition.Name#>Service <# Definition.NameLow#>Service,
<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))
#>		I<# definition.Name#>Service <# definition.NameLow#>Service,
<#end#>		IFilterService filterService,
		IFavoriteService favoriteService,
		ILogger<<# Definition.Name#>sModel> log)
	{
		_<# Definition.NameLow#>Service = <# Definition.NameLow#>Service;
<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))
#>		_<# definition.NameLow#>Service = <# definition.NameLow#>Service;
<#end#>		_filterService = filterService;
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
			var favoritesTask = _favoriteService.GetItemsAsync(ItemType.<# Definition.Name#>);

			Items = HasActiveFilter
				? [.. await _<# Definition.NameLow#>Service.GetItemsAsync(activeFilter).ConfigureAwait(false)]
				: [.. await _<# Definition.NameLow#>Service.GetItemsAsync().ConfigureAwait(false)];

			var favoriteItems = await favoritesTask.ConfigureAwait(false);
			FavoriteItemIds = [.. favoriteItems.Select(item => item.ItemId)];
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error loading <# Definition.Name#>s.");
			TempData["ErrorTitle"] = <# Definition.Name#>Loc.<# Definition.Name#>s;
			TempData["ErrorMessage"] = ex.Message;
		}

		if (Request.Headers.ContainsKey("HX-Request"))
		{
			return Partial("_<# Definition.Name#>List", this);
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
				TargetType = ItemType.<# Definition.Name#>,
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
			_log.LogError(ex, "Error saving filter for <# Definition.Name#>s.");
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
			_log.LogError(ex, "Error deleting filter for <# Definition.Name#>s.");
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
				TargetType = ItemType.<# Definition.Name#>,
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
			_log.LogError(ex, "Error setting default filter for <# Definition.Name#>s.");
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
			_log.LogError(ex, "Error clearing default filter for <# Definition.Name#>s.");
			TempData["ErrorTitle"] = FilterLoc.Filters;
			TempData["ErrorMessage"] = ex.Message;
			return CreateFilterManagementRedirect(filterState, namedFilter);
		}
	}

	private async Task<Filter[]> LoadFilterContextAsync()
	{
		<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))
#>		<# definition.Name#>s = await _<# definition.NameLow#>Service.GetItemsAsync();
<#end#>		AvailableFilterFields = BuildFilterFieldOptions(<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))#><# definition.Name#>s<#if definition != Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n)).Last()#>, <#end#><#end#>);
		var filters = await _filterService.GetItemsAsync(ItemType.<# Definition.Name#>).ConfigureAwait(false);
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
			_log.LogWarning(ex, "Ignoring invalid filter state for <# Definition.Name#>s.");
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

	private IReadOnlyList<FilterFieldOption> BuildFilterFieldOptions(<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))#>IReadOnlyList<<# definition.Name#>ListItemModel> <# definition.NameLow#>s<#if definition != Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n)).Last()#>, <#end#><#end#>)
	{
		return
		[
<#for entry in Entries.Where(e => !(e.IsKey || e.IsArray || e.IsOwnedType))
#>			new FilterFieldOption
			{
				Key = "<#cs Write(GetFieldPropertyName(entry.Field, entry.IsReference))#>",
				Label = <# Definition.Name#>Loc.<# entry.Field#>,
				Kind = FilterFieldKind.<#cs Write(GetFilterFieldKind(entry.EntryType, entry.IsReference, entry.IsEnumeration))#>,
				Operators = <#if entry.EntryType == "string"#>FilterOperatorCatalog.CreateTextOptions()<#elseif IsComparableType(entry.EntryType)#>FilterOperatorCatalog.CreateComparableOptions(<#cs Write(AllowsEmptyFilter(entry.IsNullable, entry.EntryType) ? "true" : "false")#>)<#else#>FilterOperatorCatalog.CreateEqualityOptions(<#cs Write(AllowsEmptyFilter(entry.IsNullable, entry.EntryType) ? "true" : "false")#>)<#end#>,
				Values = <#if entry.IsReference
#>[.. <# entry.ReferenceTypeLow#>s.Select(item => new FilterValueOption
				{
					Value = item.Id.ToString(),
					Label = item.DisplayText,
				})]<#elseif entry.IsEnumeration
#>[.. Enum.GetValues<<# entry.EntryType#>>().Select(value => new FilterValueOption
				{
					Value = value.ToString(),
					Label = value == <# entry.EntryType#>.None ? CommonLoc.NothingSelected : value.ToString(),
				})]<#elseif entry.EntryType == "bool"
#>[new FilterValueOption { Value = bool.TrueString, Label = bool.TrueString }, new FilterValueOption { Value = bool.FalseString, Label = bool.FalseString }]<#else#>[]<#end#>,
			},
<#end#>		];
	}
}
<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>
