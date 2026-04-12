using Najlot.Map;
using Najlot.Map.Attributes;
using <# Project.Namespace#>.Client.Data.GlobalSearch;

namespace <# Project.Namespace#>.ClientBase.GlobalSearch;

[Mapping]
internal sealed partial class GlobalSearchViewModelMappings
{
	public static partial void MapToViewModel(IMap map, GlobalSearchItemModel from, GlobalSearchItemViewModel to);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>