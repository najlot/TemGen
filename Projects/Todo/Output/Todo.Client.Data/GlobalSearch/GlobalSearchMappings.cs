using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Contracts.GlobalSearch;

namespace Todo.Client.Data.GlobalSearch;

[Mapping]
internal sealed partial class GlobalSearchMappings
{
	public static partial void MapToModel(IMap map, GlobalSearchItem from, GlobalSearchItemModel to);
}
