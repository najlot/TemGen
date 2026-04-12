using Najlot.Map;
using Najlot.Map.Attributes;
using <# Project.Namespace#>.Contracts.GlobalSearch;

namespace <# Project.Namespace#>.Client.Data.GlobalSearch;

[Mapping]
internal sealed partial class GlobalSearchMappings
{
	public static partial void MapToModel(IMap map, GlobalSearchItem from, GlobalSearchItemModel to);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>