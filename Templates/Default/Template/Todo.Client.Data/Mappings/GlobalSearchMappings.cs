using Najlot.Map;
using Najlot.Map.Attributes;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Client.Data.Mappings;

[Mapping]
internal sealed partial class GlobalSearchMappings
{
	public static partial void MapToModel(IMap map, GlobalSearchItem from, GlobalSearchItemModel to);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>