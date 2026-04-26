using Najlot.Map;
using Najlot.Map.Attributes;
using <# Project.Namespace#>.Contracts.Filters;

namespace <# Project.Namespace#>.ClientBase.Filters;

[Mapping]
internal sealed partial class EntityFilterMappings
{
	public static partial void MapToFilterCondition(IMap map, FilterCondition from, FilterCondition to);

	public static void MapToEntityFilter(IMap map, Filter from, EntityFilter to)
	{
		to.Conditions = map.From<FilterCondition>(from.Conditions).ToList<FilterCondition>();
	}

	public static void MapToEntityFilter(IMap map, EntityFilter from, EntityFilter to)
	{
		to.Conditions = map.From<FilterCondition>(from.Conditions).ToList<FilterCondition>();
	}

	[MapIgnoreProperty(nameof(to.Conditions))]
	private static partial void MapPartialToFilter(IMap map, Filter from, Filter to);

	public static void MapToFilter(IMap map, Filter from, Filter to)
	{
		MapPartialToFilter(map, from, to);
		to.Conditions = map.From<FilterCondition>(from.Conditions).ToList<FilterCondition>();
	}

	[MapValidateSource]
	public static void MapToFilter(IMap map, EntityFilter from, Filter to)
	{
		to.Conditions = map.From<FilterCondition>(from.Conditions).ToList<FilterCondition>();
	}

	public static void MapViewModelToCondition(IMap map, FilterConditionViewModel from, FilterCondition to)
	{
		to.Field = from.SelectedFieldKey;
		to.Operator = from.SelectedOperator;
		to.Value = from.RequiresValue ? string.IsNullOrWhiteSpace(from.Value) ? null : from.Value : null;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>