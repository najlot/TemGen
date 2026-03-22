using Najlot.Map;
using Najlot.Map.Attributes;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Contracts.Events;

namespace <# Project.Namespace#>.Client.Data.Mappings;

[Mapping]
internal sealed partial class TrashMappings
{
	public static partial void MapToModel(IMap map, TrashItem from, TrashItemModel to);
	public static partial void MapToModel(IMap map, TrashItemCreated from, TrashItemModel to);
	public static partial void MapToModel(IMap map, TrashItemUpdated from, TrashItemModel to);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>