using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.Models;
using Todo.Contracts;

namespace Todo.Client.Data.Mappings;

[Mapping]
internal sealed partial class GlobalSearchMappings
{
	public static partial void MapToModel(IMap map, GlobalSearchItem from, GlobalSearchItemModel to);
}
