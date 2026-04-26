using Najlot.Map;
using Najlot.Map.Attributes;
using System.Linq.Expressions;
using Todo.Contracts.Filters;

namespace Todo.Service.Features.Filters;

[Mapping]
internal partial class FilterMappings
{
	[MapIgnoreProperty(nameof(to.Conditions))]
	[PostMap(nameof(PostMapToModel))]
	public static partial void MapToModel(IMap map, CreateFilter from, FilterModel to);
	private static void PostMapToModel(IMap map, CreateFilter from, FilterModel to)
	{
		map.From<FilterCondition>(from.Conditions).ToList(to.Conditions);
	}

	[MapIgnoreProperty(nameof(to.Conditions))]
	[PostMap(nameof(PostMapToModel))]
	public static partial void MapToModel(IMap map, UpdateFilter from, FilterModel to);
	private static void PostMapToModel(IMap map, UpdateFilter from, FilterModel to)
	{
		map.From<FilterCondition>(from.Conditions).ToList(to.Conditions);
	}

	public static partial void MapConditions(IMap map, FilterCondition from, FilterCondition to);

	public static Expression<Func<FilterModel, Filter>> GetToFilterExpression()
	{
		return from => new Filter
		{
			Id = from.Id,
			TargetType = from.TargetType,
			Name = from.Name,
			IsDefault = from.IsDefault,
			Conditions = from.Conditions,
		};
	}
}
