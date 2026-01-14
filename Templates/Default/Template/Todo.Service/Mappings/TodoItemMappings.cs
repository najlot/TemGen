using Najlot.Map;
using Najlot.Map.Attributes;
using System;
using System.Linq.Expressions;
using <#cs Write(Project.Namespace)#>.Contracts;
using <#cs Write(Project.Namespace)#>.Contracts.Commands;
using <#cs Write(Project.Namespace)#>.Contracts.Events;
using <#cs Write(Project.Namespace)#>.Contracts.ListItems;
using <#cs Write(Project.Namespace)#>.Service.Model;

namespace <#cs Write(Project.Namespace)#>.Service.Mappings;

[Mapping]
internal partial class <#cs Write(Definition.Name)#>Mappings
{
	public static <#cs Write(Definition.Name)#>Created MapToCreated(IMap map, <#cs Write(Definition.Name)#>Model from) =>
		new(from.Id,
<#cs WriteCtorMapping("", "", MapArrayStrategy.LeaveAsIs)#>);

	public static <#cs Write(Definition.Name)#>Updated MapToUpdated(IMap map, <#cs Write(Definition.Name)#>Model from) =>
		new(from.Id,
<#cs WriteCtorMapping("", "", MapArrayStrategy.LeaveAsIs)#>);

	public static partial void MapToModel(IMap map, Create<#cs Write(Definition.Name)#> from, <#cs Write(Definition.Name)#>Model to);

	public static void MapToModel(IMap map, Update<#cs Write(Definition.Name)#> from, <#cs Write(Definition.Name)#>Model to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("", "", arrayStrategy: MapArrayStrategy.MapInto)#>	}

	public static partial void MapToModel(IMap map, <#cs Write(Definition.Name)#>Model from, <#cs Write(Definition.Name)#> to);

	public static Expression<Func<<#cs Write(Definition.Name)#>Model, <#cs Write(Definition.Name)#>ListItem>> GetListItemExpression()
	{
		return from => new <#cs Write(Definition.Name)#>ListItem
		{
			Id = from.Id,
<#cs
string tabs = "\t\t\t";

foreach(var entry in Entries
	.Where(e => !e.IsArray && !e.IsOwnedType && !e.IsReference)
	.Take(2))
{
	WriteLine($"{tabs}{entry.Field} = from.{entry.Field},");
}

Result = Result.TrimEnd(',', '\n', '\r');
#>
		};
	}

	public static void MapToModel(IMap map, <#cs Write(Definition.Name)#>Model from, <#cs Write(Definition.Name)#>ListItem to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("Model", "", 2, onlySimple: true)#>	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>