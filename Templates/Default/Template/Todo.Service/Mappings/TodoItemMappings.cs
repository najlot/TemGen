using Najlot.Map;
using Najlot.Map.Attributes;
using System.Linq.Expressions;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Contracts.Commands;
using <# Project.Namespace#>.Contracts.Events;
using <# Project.Namespace#>.Contracts.ListItems;
using <# Project.Namespace#>.Service.Model;

namespace <# Project.Namespace#>.Service.Mappings;

[Mapping]
internal partial class <# Definition.Name#>Mappings
{
	public static partial void MapToCreated(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>Created to);

	public static partial void MapToUpdated(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>Updated to);

	[MapIgnoreProperty(nameof(to.DeletedAt))]
	public static partial void MapToModel(IMap map, Create<# Definition.Name#> from, <# Definition.Name#>Model to);

<#if Entries.Any(e => e.IsArray)
#>	[MapIgnoreProperty(nameof(to.DeletedAt))]
<#for entry in Entries.Where(e => e.IsArray)
#>	[MapIgnoreProperty(nameof(to.<# entry.Field#>))]
<#end#>	private static partial void MapPartialToModel(IMap map, Update<# Definition.Name#> from, <# Definition.Name#>Model to);
	public static void MapToModel(IMap map, Update<# Definition.Name#> from, <# Definition.Name#>Model to)
	{
		MapPartialToModel(map, from, to);<#if Entries.Any(e => e.IsArray)#>
<#end#>
<#for entry in Entries.Where(e => e.IsArray)
#>		to.<# entry.Field#> = map.From<<# entry.EntryType#>>(from.<# entry.Field#>).ToList(to.<# entry.Field#>);
<#end#>	}
	<#else#>	[MapIgnoreProperty(nameof(to.DeletedAt))]
	public static partial void MapToModel(IMap map, Update<# Definition.Name#> from, <# Definition.Name#>Model to);
<#end#>
	public static partial void MapToModel(IMap map, <# Definition.Name#>Model from, <# Definition.Name#> to);

	public static Expression<Func<<# Definition.Name#>Model, <# Definition.Name#>ListItem>> GetListItemExpression()
	{
		return from => new <# Definition.Name#>ListItem
		{
			Id = from.Id,
<#for entry in Entries
	.Where(e => !e.IsArray && !e.IsOwnedType && !e.IsReference)
	.Take(2)
#>			<# entry.Field#> = from.<# entry.Field#>,
<#end#><#cs Result = Result.TrimEnd('\r', '\n', ',')#>
		};
	}

	public static partial void MapToModel(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>ListItem to);
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>