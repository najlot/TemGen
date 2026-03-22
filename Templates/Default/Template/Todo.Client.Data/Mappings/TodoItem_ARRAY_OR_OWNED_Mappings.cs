using Najlot.Map;
using Najlot.Map.Attributes;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Client.Data.Mappings;

[Mapping]
internal sealed partial class <# Definition.Name#>Mappings
{
	public static partial void MapFromModel(IMap map, <# Definition.Name#>Model from, <# Definition.Name#> to);

	public static partial void MapToModel(IMap map, <# Definition.Name#> from, <# Definition.Name#>Model to);

	public static partial void MapFromModelToModel(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>Model to);
}<#cs
SetOutputPath(!(Definition.IsOwnedType || Definition.IsArray));
RelativePath = RelativePath.Replace("_ARRAY_OR_OWNED_", "");
#>