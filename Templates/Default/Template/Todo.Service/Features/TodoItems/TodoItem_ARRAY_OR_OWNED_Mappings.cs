using Najlot.Map;
using Najlot.Map.Attributes;
using <# Project.Namespace#>.Contracts.<#if
Definition.IsOwnedType || Definition.IsArray
#><#cs Write(GetChildFeatureFolderName(Definition.Name))#><#else#><# Definition.Name#>s<#end#>;

namespace <# Project.Namespace#>.Service.Features.<# Definition.Name#>s;

[Mapping]
internal partial class <# Definition.Name#>Mappings
{
<#if Definition.IsArray
#>	[MapIgnoreProperty(nameof(to.Id))]
<#end#>	public static partial void Map(IMap map, <# Definition.Name#> from, <# Definition.Name#> to);
}<#cs
SetOutputPath(!(Definition.IsOwnedType || Definition.IsArray));

if (Definition.IsOwnedType || Definition.IsArray)
{
	RelativePath = RelativePath.Replace("_ARRAY_OR_OWNED_", "");
	MoveChildToFeatureFolder();
}
#>