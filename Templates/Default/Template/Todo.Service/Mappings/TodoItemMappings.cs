using Najlot.Map;
using <#cs Write(Project.Namespace)#>.Contracts;
using <#cs Write(Project.Namespace)#>.Contracts.Commands;
using <#cs Write(Project.Namespace)#>.Contracts.Events;
using <#cs Write(Project.Namespace)#>.Contracts.ListItems;
using <#cs Write(Project.Namespace)#>.Service.Model;

namespace <#cs Write(Project.Namespace)#>.Service.Mappings;

internal class <#cs Write(Definition.Name)#>Mappings
{
	public <#cs Write(Definition.Name)#>Created MapToCreated(IMap map, <#cs Write(Definition.Name)#>Model from) =>
		new(from.Id,
<#cs WriteCtorMapping("", "", MapArrayStrategy.LeaveAsIs)#>);

	public <#cs Write(Definition.Name)#>Updated MapToUpdated(IMap map, <#cs Write(Definition.Name)#>Model from) =>
		new(from.Id,
<#cs WriteCtorMapping("", "", MapArrayStrategy.LeaveAsIs)#>);

	public void MapToModel(IMap map, Create<#cs Write(Definition.Name)#> from, <#cs Write(Definition.Name)#>Model to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("", "", arrayStrategy: MapArrayStrategy.LeaveAsIs)#>	}

	public void MapToModel(IMap map, Update<#cs Write(Definition.Name)#> from, <#cs Write(Definition.Name)#>Model to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("", "", arrayStrategy: MapArrayStrategy.MapInto)#>	}

	public void MapToModel(IMap map, <#cs Write(Definition.Name)#>Model from, <#cs Write(Definition.Name)#> to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("", "", arrayStrategy: MapArrayStrategy.LeaveAsIs)#>	}

	public void MapToModel(IMap map, <#cs Write(Definition.Name)#>Model from, <#cs Write(Definition.Name)#>ListItem to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("Model", "", 2, onlySimple: true)#>	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>