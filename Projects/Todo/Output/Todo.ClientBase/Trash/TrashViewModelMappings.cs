using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.Trash;
using Todo.Contracts.Trash;

namespace Todo.ClientBase.Trash;

[Mapping]
internal sealed partial class TrashViewModelMappings
{
	public static partial void MapToViewModel(IMap map, TrashItemModel from, TrashItemViewModel to);

	public static partial void MapToViewModel(IMap map, TrashItemCreated from, TrashItemViewModel to);

	public static partial void MapToViewModel(IMap map, TrashItemUpdated from, TrashItemViewModel to);
}
