using Najlot.Map;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Contracts;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Mappings;

internal sealed class <#cs Write(Definition.Name)#>Mappings
{
	public void MapFromModel(IMap map, <#cs Write(Definition.Name)#>Model from, <#cs Write(Definition.Name)#> to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("", "Model")#>	}

	public void MapToModel(IMap map, <#cs Write(Definition.Name)#> from, <#cs Write(Definition.Name)#>Model to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping()#>	}

	public void MapFromModelToModel(IMap map, <#cs Write(Definition.Name)#>Model from, <#cs Write(Definition.Name)#>Model to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping()#>	}
}<#cs
SetOutputPath(!(Definition.IsOwnedType || Definition.IsArray));
RelativePath = RelativePath.Replace("_ARRAY_OR_OWNED_", "");
#>