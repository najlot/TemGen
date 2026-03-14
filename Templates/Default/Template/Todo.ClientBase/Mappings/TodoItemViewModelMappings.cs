using Najlot.Map;
using Najlot.Map.Attributes;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;
using <#cs Write(Project.Namespace)#>.Contracts;
using <#cs Write(Project.Namespace)#>.Contracts.Events;

namespace <#cs Write(Project.Namespace)#>.ClientBase.Mappings;

[Mapping]
internal sealed partial class <#cs Write(Definition.Name)#>ViewModelMappings
{
<#cs foreach(var entry in Entries.Where(e => e.IsArray))
{
	WriteLine($"	[MapIgnoreProperty(nameof(to.{entry.Field}))]");
}#>	private static partial void MapPartialToViewModel(IMap map, <#cs Write(Definition.Name)#>Updated from, <#cs Write(Definition.Name)#>ViewModel to);

	[MapValidateSource]
	public static void MapToViewModel(IMap map, <#cs Write(Definition.Name)#>Updated from, <#cs Write(Definition.Name)#>ViewModel to)
	{
		MapPartialToViewModel(map, from, to);
<#cs
if (Entries.Any(e => e.IsArray)) WriteLine("");
foreach(var entry in Entries.Where(e => e.IsArray))
{
	WriteLine($"		to.{entry.Field} = [.. map.From<{entry.EntryType}>(from.{entry.Field}).To<{entry.EntryType}ViewModel>()];");
	WriteLine("");
	WriteLine($"		foreach (var e in to.{entry.Field}) e.ParentId = from.Id;");
}
#>	}

	public static partial void MapToModel(IMap map, <#cs Write(Definition.Name)#>ViewModel from, <#cs Write(Definition.Name)#>Model to);

<#cs foreach(var entry in Entries.Where(e => e.IsArray))
{
	WriteLine($"	[MapIgnoreProperty(nameof(to.{entry.Field}))]");
}#>	private static partial void MapPartialToViewModel(IMap map, <#cs Write(Definition.Name)#>Model from, <#cs Write(Definition.Name)#>ViewModel to);

	[MapValidateSource]
	public static void MapToViewModel(IMap map, <#cs Write(Definition.Name)#>Model from, <#cs Write(Definition.Name)#>ViewModel to)
	{
		MapPartialToViewModel(map, from, to);
<#cs
if (Entries.Any(e => e.IsArray)) WriteLine("");
foreach(var entry in Entries.Where(e => e.IsArray))
{
	WriteLine($"		to.{entry.Field} = [.. map.From<{entry.EntryType}Model>(from.{entry.Field}).To<{entry.EntryType}ViewModel>()];");
	WriteLine("");
	WriteLine($"		foreach (var e in to.{entry.Field}) e.ParentId = from.Id;");
}
#>	}

	[MapIgnoreProperty(nameof(to.ChangeVisitor))]
	public static partial void MapToViewModel(IMap map, <#cs Write(Definition.Name)#>ListItemModel from, <#cs Write(Definition.Name)#>ListItemViewModel to);

	[MapIgnoreProperty(nameof(to.ChangeVisitor))]
	public static partial void MapToViewModel(IMap map, <#cs Write(Definition.Name)#>Created from, <#cs Write(Definition.Name)#>ListItemViewModel to);

	[MapIgnoreProperty(nameof(to.ChangeVisitor))]
	public static partial void MapToViewModel(IMap map, <#cs Write(Definition.Name)#>Updated from, <#cs Write(Definition.Name)#>ListItemViewModel to);
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>