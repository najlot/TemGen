using Najlot.Map;
using Najlot.Map.Attributes;
using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Service.Mappings;

[Mapping]
internal partial class <# Definition.Name#>Mappings
{
	[MapIgnoreProperty(nameof(to.Id))] // Do not map the Id property as it makes problems with entity tracking in EF Core
	public static partial void Map(IMap map, <# Definition.Name#> from, <# Definition.Name#> to);
}<#cs
SetOutputPath(!(Definition.IsOwnedType || Definition.IsArray));
RelativePath = RelativePath.Replace("_ARRAY_OR_OWNED_", "");
#>