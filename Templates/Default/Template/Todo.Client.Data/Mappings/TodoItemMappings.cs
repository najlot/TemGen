using Najlot.Map;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Contracts;
using <#cs Write(Project.Namespace)#>.Contracts.Commands;
using <#cs Write(Project.Namespace)#>.Contracts.Events;
using <#cs Write(Project.Namespace)#>.Contracts.ListItems;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Mappings;

internal sealed class <#cs Write(Definition.Name)#>Mappings
{
	public Create<#cs Write(Definition.Name)#> MapToCreate(IMap map, <#cs Write(Definition.Name)#>Model from) =>
		new(from.Id,
<#cs WriteCtorMapping("Model", "")#>);

	public <#cs Write(Definition.Name)#>Created MapToCreated(IMap map, <#cs Write(Definition.Name)#>Model from) =>
		new(from.Id,
<#cs WriteCtorMapping("Model", "")#>);

	public Update<#cs Write(Definition.Name)#> MapToUpdate(IMap map, <#cs Write(Definition.Name)#>Model from) =>
		new(from.Id,
<#cs WriteCtorMapping("Model", "")#>);

	public <#cs Write(Definition.Name)#>Updated MapToUpdated(IMap map, <#cs Write(Definition.Name)#>Model from) =>
		new(from.Id,
<#cs WriteCtorMapping("Model", "")#>);

	public void MapToModel(IMap map, <#cs Write(Definition.Name)#>Created from, <#cs Write(Definition.Name)#>ListItemModel to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("", "", 2, onlySimple: true)#>	}

	public void MapToModel(IMap map, <#cs Write(Definition.Name)#>Updated from, <#cs Write(Definition.Name)#>ListItemModel to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("", "", 2, onlySimple: true)#>	}

	public void MapToModel(IMap map, <#cs Write(Definition.Name)#>ListItem from, <#cs Write(Definition.Name)#>ListItemModel to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("", "", 2, onlySimple: true)#>	}

	public void MapModelToModel(IMap map, <#cs Write(Definition.Name)#>Model from, <#cs Write(Definition.Name)#>ListItemModel to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("", "", 2, onlySimple: true)#>	}

	public void MapToModel(IMap map, <#cs Write(Definition.Name)#> from, <#cs Write(Definition.Name)#>Model to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("", "Model")#>	}

	public void MapToModel(IMap map, <#cs Write(Definition.Name)#>Updated from, <#cs Write(Definition.Name)#>Model to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("", "Model")#>	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>