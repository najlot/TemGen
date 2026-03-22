using Najlot.Map;
using Najlot.Map.Attributes;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.ClientBase.ViewModel;

namespace <# Project.Namespace#>.ClientBase.Mappings;

[Mapping]
internal sealed partial class GlobalSearchViewModelMappings
{
	[MapIgnoreProperty(nameof(to.ChangeVisitor))]
	public static partial void MapToViewModel(IMap map, GlobalSearchItemModel from, GlobalSearchItemViewModel to);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>