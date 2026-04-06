using Najlot.Map;
using Najlot.Map.Attributes;
using System.Linq.Expressions;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Contracts.Commands;
using <# Project.Namespace#>.Contracts.Events;
using <# Project.Namespace#>.Contracts.ListItems;

namespace <# Project.Namespace#>.Service.Features.<# Definition.Name#>s;

[Mapping]
internal partial class <# Definition.Name#>Mappings
{
	public static partial void MapToCreated(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>Created to);

	public static partial void MapToUpdated(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>Updated to);

<#if Entries.Any(e => e.IsArray)
#>	[MapIgnoreProperty(nameof(to.DeletedAt))]
<#for entry in Entries.Where(e => e.IsArray)
#>	[MapIgnoreProperty(nameof(to.<# entry.Field#>))]
<#end#>	[PostMap(nameof(PostMapToModel))]
	public static partial void MapToModel(IMap map, Create<# Definition.Name#> from, <# Definition.Name#>Model to);
	private static void PostMapToModel(IMap map, Create<# Definition.Name#> from, <# Definition.Name#>Model to)
	{
<#for entry in Entries.Where(e => e.IsArray)
#>		to.<# entry.Field#> = map.From<<# entry.EntryType#>>(from.<# entry.Field#>).ToList(to.<# entry.Field#>);
<#end#>	}

	[MapIgnoreProperty(nameof(to.DeletedAt))]
<#for entry in Entries.Where(e => e.IsArray)
#>	[MapIgnoreProperty(nameof(to.<# entry.Field#>))]
<#end#>	[PostMap(nameof(PostMapToModel))]
	public static partial void MapToModel(IMap map, Update<# Definition.Name#> from, <# Definition.Name#>Model to);

	private static void PostMapToModel(IMap map, Update<# Definition.Name#> from, <# Definition.Name#>Model to)
	{
<#for entry in Entries.Where(e => e.IsArray)
#>		to.<# entry.Field#> = map.From<<# entry.EntryType#>>(from.<# entry.Field#>).ToList(to.<# entry.Field#>);
<#end#>	}
<#else#>	[MapIgnoreProperty(nameof(to.DeletedAt))]
	public static partial void MapToModel(IMap map, Create<# Definition.Name#> from, <# Definition.Name#>Model to);

	[MapIgnoreProperty(nameof(to.DeletedAt))]
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