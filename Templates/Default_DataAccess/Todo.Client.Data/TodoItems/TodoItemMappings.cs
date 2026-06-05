using Najlot.Map;
using Najlot.Map.Attributes;
using <# Project.Namespace#>.Contracts.<# Definition.Name#>s;

namespace <# Project.Namespace#>.Client.Data.<# Definition.Name#>s;

[Mapping]
internal sealed partial class <# Definition.Name#>Mappings
{
<#if Entries.Any(e => e.IsArray)#><#for entry in Entries.Where(e => e.IsArray)
#>	[MapIgnoreProperty(nameof(to.<# entry.Field#>))]
<#end#>	private static partial void MapPartialToCreate(IMap map, <# Definition.Name#>Model from, Create<# Definition.Name#> to);

	public static void MapToCreate(IMap map, <# Definition.Name#>Model from, Create<# Definition.Name#> to)
	{
		MapPartialToCreate(map, from, to);
<#for entry in Entries.Where(e => e.IsArray)
#>		to.<# entry.Field#> = map.From<<# entry.EntryType#>Model>(from.<# entry.Field#>).ToList<<# entry.EntryType#>>();
<#end#>	}

<#for entry in Entries.Where(e => e.IsArray)
#>	[MapIgnoreProperty(nameof(to.<# entry.Field#>))]
<#end#>	private static partial void MapPartialToCreated(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>Created to);

	public static void MapToCreated(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>Created to)
	{
		MapPartialToCreated(map, from, to);
<#for entry in Entries.Where(e => e.IsArray)
#>		to.<# entry.Field#> = map.From<<# entry.EntryType#>Model>(from.<# entry.Field#>).ToList<<# entry.EntryType#>>();
<#end#>	}

<#for entry in Entries.Where(e => e.IsArray)
#>	[MapIgnoreProperty(nameof(to.<# entry.Field#>))]
<#end#>	private static partial void MapPartialToUpdate(IMap map, <# Definition.Name#>Model from, Update<# Definition.Name#> to);

	public static void MapToUpdate(IMap map, <# Definition.Name#>Model from, Update<# Definition.Name#> to)
	{
		MapPartialToUpdate(map, from, to);
<#for entry in Entries.Where(e => e.IsArray)
#>		to.<# entry.Field#> = map.From<<# entry.EntryType#>Model>(from.<# entry.Field#>).ToList<<# entry.EntryType#>>();
<#end#>	}

<#for entry in Entries.Where(e => e.IsArray)
#>	[MapIgnoreProperty(nameof(to.<# entry.Field#>))]
<#end#>	private static partial void MapPartialToUpdated(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>Updated to);

	public static void MapToUpdated(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>Updated to)
	{
		MapPartialToUpdated(map, from, to);
<#for entry in Entries.Where(e => e.IsArray)
#>		to.<# entry.Field#> = map.From<<# entry.EntryType#>Model>(from.<# entry.Field#>).ToList<<# entry.EntryType#>>();
<#end#>	}
<#else#>	public static partial void MapToCreate(IMap map, <# Definition.Name#>Model from, Create<# Definition.Name#> to);

	public static partial void MapToCreated(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>Created to);

	public static partial void MapToUpdate(IMap map, <# Definition.Name#>Model from, Update<# Definition.Name#> to);

	public static partial void MapToUpdated(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>Updated to);
<#end#>
	public static partial void MapToModel(IMap map, <# Definition.Name#>Created from, <# Definition.Name#>ListItemModel to);

	public static partial void MapToModel(IMap map, <# Definition.Name#>Updated from, <# Definition.Name#>ListItemModel to);

	public static partial void MapToModel(IMap map, <# Definition.Name#>ListItem from, <# Definition.Name#>ListItemModel to);

	public static partial void MapModelToModel(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>ListItemModel to);

	public static partial void MapToModel(IMap map, <# Definition.Name#> from, <# Definition.Name#>Model to);

	public static partial void MapToModel(IMap map, <# Definition.Name#>Updated from, <# Definition.Name#>Model to);
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>