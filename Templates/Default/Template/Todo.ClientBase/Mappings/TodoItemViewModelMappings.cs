using Najlot.Map;
using Najlot.Map.Attributes;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.ClientBase.ViewModels;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Contracts.Events;

namespace <# Project.Namespace#>.ClientBase.Mappings;

[Mapping]
internal sealed partial class <# Definition.Name#>ViewModelMappings
{
<#for entry in Entries.Where(e => e.IsArray)
#>	[MapIgnoreProperty(nameof(to.<# entry.Field#>))]
<#end#>	private static partial void MapPartialToViewModel(IMap map, <# Definition.Name#>Updated from, <# Definition.Name#>ViewModel to);

	[MapValidateSource]
	public static void MapToViewModel(IMap map, <# Definition.Name#>Updated from, <# Definition.Name#>ViewModel to)
	{
		MapPartialToViewModel(map, from, to);<#if Entries.Any(e => e.IsArray)#>
<#end#>
<#for entry in Entries.Where(e => e.IsArray)
#>		to.<# entry.Field#> = [.. map.From<<# entry.EntryType#>>(from.<# entry.Field#>).To<<# entry.EntryType#>ViewModel>()];
<#end#>	}

	public static partial void MapToModel(IMap map, <# Definition.Name#>ViewModel from, <# Definition.Name#>Model to);

<#for entry in Entries.Where(e => e.IsArray)
#>	[MapIgnoreProperty(nameof(to.<# entry.Field#>))]
<#end#>	private static partial void MapPartialToViewModel(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>ViewModel to);

	[MapValidateSource]
	public static void MapToViewModel(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>ViewModel to)
	{
		MapPartialToViewModel(map, from, to);<#if Entries.Any(e => e.IsArray)#>
<#end#>
<#for entry in Entries.Where(e => e.IsArray)
#>		to.<# entry.Field#> = [.. map.From<<# entry.EntryType#>Model>(from.<# entry.Field#>).To<<# entry.EntryType#>ViewModel>()];
<#end#>	}

	[MapIgnoreProperty(nameof(to.ChangeVisitor))]
	public static partial void MapToViewModel(IMap map, <# Definition.Name#>ListItemModel from, <# Definition.Name#>ListItemViewModel to);

	[MapIgnoreProperty(nameof(to.ChangeVisitor))]
	public static partial void MapToViewModel(IMap map, <# Definition.Name#>Created from, <# Definition.Name#>ListItemViewModel to);

	[MapIgnoreProperty(nameof(to.ChangeVisitor))]
	public static partial void MapToViewModel(IMap map, <# Definition.Name#>Updated from, <# Definition.Name#>ListItemViewModel to);
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>