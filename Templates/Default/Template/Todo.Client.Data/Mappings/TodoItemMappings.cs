using Najlot.Map;
using Najlot.Map.Attributes;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Contracts;
using <#cs Write(Project.Namespace)#>.Contracts.Commands;
using <#cs Write(Project.Namespace)#>.Contracts.Events;
using <#cs Write(Project.Namespace)#>.Contracts.ListItems;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Mappings;

[Mapping]
internal sealed partial class <#cs Write(Definition.Name)#>Mappings
{
	public static Create<#cs Write(Definition.Name)#> MapToCreate(IMap map, <#cs Write(Definition.Name)#>Model from) =>
		new(from.Id,
<#cs WriteCtorMapping("Model", "")#>);

	public static <#cs Write(Definition.Name)#>Created MapToCreated(IMap map, <#cs Write(Definition.Name)#>Model from) =>
		new(from.Id,
<#cs WriteCtorMapping("Model", "")#>);

	public static Update<#cs Write(Definition.Name)#> MapToUpdate(IMap map, <#cs Write(Definition.Name)#>Model from) =>
		new(from.Id,
<#cs WriteCtorMapping("Model", "")#>);

	public static <#cs Write(Definition.Name)#>Updated MapToUpdated(IMap map, <#cs Write(Definition.Name)#>Model from) =>
		new(from.Id,
<#cs WriteCtorMapping("Model", "")#>);

	public static partial void MapToModel(IMap map, <#cs Write(Definition.Name)#>Created from, <#cs Write(Definition.Name)#>ListItemModel to);

	public static partial void MapToModel(IMap map, <#cs Write(Definition.Name)#>Updated from, <#cs Write(Definition.Name)#>ListItemModel to);

	public static partial void MapToModel(IMap map, <#cs Write(Definition.Name)#>ListItem from, <#cs Write(Definition.Name)#>ListItemModel to);

	public static partial void MapModelToModel(IMap map, <#cs Write(Definition.Name)#>Model from, <#cs Write(Definition.Name)#>ListItemModel to);

	public static partial void MapToModel(IMap map, <#cs Write(Definition.Name)#> from, <#cs Write(Definition.Name)#>Model to);

	public static partial void MapToModel(IMap map, <#cs Write(Definition.Name)#>Updated from, <#cs Write(Definition.Name)#>Model to);
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>