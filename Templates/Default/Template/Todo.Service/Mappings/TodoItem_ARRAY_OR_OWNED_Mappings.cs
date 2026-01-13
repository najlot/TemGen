using Najlot.Map;
using <#cs Write(Project.Namespace)#>.Contracts;

namespace <#cs Write(Project.Namespace)#>.Service.Mappings;

internal class <#cs Write(Definition.Name)#>Mappings
{
	public void Map(IMap map, <#cs Write(Definition.Name)#> from, <#cs Write(Definition.Name)#> to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("", "")#>	}
}<#cs
SetOutputPath(!(Definition.IsOwnedType || Definition.IsArray));
RelativePath = RelativePath.Replace("_ARRAY_OR_OWNED_", "");
#>