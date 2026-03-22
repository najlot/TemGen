using Najlot.Map;
using Najlot.Map.Attributes;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.ClientBase.ViewModel;

using <# Project.Namespace#>.Contracts.Events;

namespace <# Project.Namespace#>.ClientBase.Mappings;

[Mapping]
internal sealed partial class TrashViewModelMappings
{
	[MapIgnoreProperty(nameof(to.ChangeVisitor))]
	public static partial void MapToViewModel(IMap map, TrashItemModel from, TrashItemViewModel to);

	[MapIgnoreProperty(nameof(to.ChangeVisitor))]
	public static partial void MapToViewModel(IMap map, TrashItemCreated from, TrashItemViewModel to);

	[MapIgnoreProperty(nameof(to.ChangeVisitor))]
	public static partial void MapToViewModel(IMap map, TrashItemUpdated from, TrashItemViewModel to);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>