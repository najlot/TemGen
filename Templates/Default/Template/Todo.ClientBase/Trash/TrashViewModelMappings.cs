using Najlot.Map;
using Najlot.Map.Attributes;
using <# Project.Namespace#>.Client.Data.Trash;
using <# Project.Namespace#>.Contracts.Trash;

namespace <# Project.Namespace#>.ClientBase.Trash;

[Mapping]
internal sealed partial class TrashViewModelMappings
{
	public static partial void MapToViewModel(IMap map, TrashItemModel from, TrashItemViewModel to);

	public static partial void MapToViewModel(IMap map, TrashItemCreated from, TrashItemViewModel to);

	public static partial void MapToViewModel(IMap map, TrashItemUpdated from, TrashItemViewModel to);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>