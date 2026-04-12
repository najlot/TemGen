using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.GlobalSearch;

namespace Todo.ClientBase.GlobalSearch;

[Mapping]
internal sealed partial class GlobalSearchViewModelMappings
{
	public static partial void MapToViewModel(IMap map, GlobalSearchItemModel from, GlobalSearchItemViewModel to);
}
