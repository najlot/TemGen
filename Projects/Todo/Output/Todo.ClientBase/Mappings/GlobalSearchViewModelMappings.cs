using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.Models;
using Todo.ClientBase.ViewModels;

namespace Todo.ClientBase.Mappings;

[Mapping]
internal sealed partial class GlobalSearchViewModelMappings
{
	[MapIgnoreProperty(nameof(to.ChangeVisitor))]
	public static partial void MapToViewModel(IMap map, GlobalSearchItemModel from, GlobalSearchItemViewModel to);
}
