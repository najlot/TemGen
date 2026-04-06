using Najlot.Map;
using Najlot.Map.Attributes;
using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Service.Features.<# Definition.Name#>s;

[Mapping]
internal partial class <# Definition.Name#>Mappings
{
<#if Definition.IsArray
#>	[MapIgnoreProperty(nameof(to.Id))]
<#end#>	public static partial void Map(IMap map, <# Definition.Name#> from, <# Definition.Name#> to);
}<#cs
SetOutputPath(!(Definition.IsOwnedType || Definition.IsArray));
RelativePath = RelativePath.Replace("_ARRAY_OR_OWNED_", "");

if (!string.IsNullOrEmpty(RelativePath))
{
	var usages = Definitions.Where(d => d.Entries.Any(p => p.EntryTypeLow == Definition.NameLow)).ToList();
	if (usages.Count == 1)
	{
		var usage = usages.First();
		var lastIndex = RelativePath.LastIndexOfAny(['\\', '/']);
		var preLastIndex = RelativePath.LastIndexOfAny(['\\', '/'], lastIndex - 1);
		var toReplace = RelativePath.Substring(preLastIndex, lastIndex - preLastIndex + 1);
		var replaced = toReplace.Replace(Definition.Name, usage.Name);
		RelativePath = RelativePath.Replace(toReplace, replaced);
	}
}
#>