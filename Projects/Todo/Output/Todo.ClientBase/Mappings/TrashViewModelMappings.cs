using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.Models;
using Todo.ClientBase.ViewModels;
using Todo.Contracts.Events;

namespace Todo.ClientBase.Mappings;

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
