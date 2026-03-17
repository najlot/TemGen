using Najlot.Map;
using Najlot.Map.Attributes;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Contracts.Commands;
using <# Project.Namespace#>.Contracts.Events;
using <# Project.Namespace#>.Contracts.ListItems;

namespace <# Project.Namespace#>.Client.Data.Mappings;

[Mapping]
internal sealed partial class <# Definition.Name#>Mappings
{
	public static Create<# Definition.Name#> MapToCreate(IMap map, <# Definition.Name#>Model from) =>
		new(from.Id,
<#cs WriteCtorMapping("Model", "")#>);

	public static <# Definition.Name#>Created MapToCreated(IMap map, <# Definition.Name#>Model from) =>
		new(from.Id,
<#cs WriteCtorMapping("Model", "")#>);

	public static Update<# Definition.Name#> MapToUpdate(IMap map, <# Definition.Name#>Model from) =>
		new(from.Id,
<#cs WriteCtorMapping("Model", "")#>);

	public static <# Definition.Name#>Updated MapToUpdated(IMap map, <# Definition.Name#>Model from) =>
		new(from.Id,
<#cs WriteCtorMapping("Model", "")#>);

	public static partial void MapToModel(IMap map, <# Definition.Name#>Created from, <# Definition.Name#>ListItemModel to);

	public static partial void MapToModel(IMap map, <# Definition.Name#>Updated from, <# Definition.Name#>ListItemModel to);

	public static partial void MapToModel(IMap map, <# Definition.Name#>ListItem from, <# Definition.Name#>ListItemModel to);

	public static partial void MapModelToModel(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>ListItemModel to);

	public static partial void MapToModel(IMap map, <# Definition.Name#> from, <# Definition.Name#>Model to);

	public static partial void MapToModel(IMap map, <# Definition.Name#>Updated from, <# Definition.Name#>Model to);
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>