using Najlot.Map;
using Najlot.Map.Attributes;
using <# Project.Namespace#>.Contracts.Trash;

namespace <# Project.Namespace#>.Client.Data.Trash;

[Mapping]
internal sealed partial class TrashMappings
{
	public static partial void MapToModel(IMap map, TrashItem from, TrashItemModel to);
	public static partial void MapToModel(IMap map, TrashItemCreated from, TrashItemModel to);
	public static partial void MapToModel(IMap map, TrashItemUpdated from, TrashItemModel to);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>