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
<#cs 
var hasArrays = Entries.Any(e => e.IsArray);
if (hasArrays) 
{
	foreach(var entry in Entries.Where(e => e.IsArray))
	{
		WriteLine($"	[MapIgnoreProperty(nameof(to.{entry.Field}))]");
	}
	WriteLine($"	private static partial void MapPartialToViewModel(IMap map, {Definition.Name}Updated from, {Definition.Name}ViewModel to);");
	WriteLine("");
	WriteLine("	[MapValidateSource]");
	WriteLine($"	public static void MapToViewModel(IMap map, {Definition.Name}Updated from, {Definition.Name}ViewModel to)");
	WriteLine("	{");
	WriteLine("		MapPartialToViewModel(map, from, to);");
	WriteLine("");
	foreach(var entry in Entries.Where(e => e.IsArray))
	{
		WriteLine($"		to.{entry.Field} = [.. map.From<{entry.EntryType}>(from.{entry.Field}).To<{entry.EntryType}ViewModel>()];");
		WriteLine("");
		WriteLine($"		foreach (var e in to.{entry.Field}) e.ParentId = from.Id;");
	}
	WriteLine("	}");
	WriteLine("");
	WriteLine($"	public static partial void MapToModel(IMap map, {Definition.Name}ViewModel from, {Definition.Name}Model to);");
	WriteLine("");
	foreach(var entry in Entries.Where(e => e.IsArray))
	{
		WriteLine($"	[MapIgnoreProperty(nameof(to.{entry.Field}))]");
	}
	WriteLine($"	private static partial void MapPartialToViewModel(IMap map, {Definition.Name}Model from, {Definition.Name}ViewModel to);");
	WriteLine("");
	WriteLine("	[MapValidateSource]");
	WriteLine($"	public static void MapToViewModel(IMap map, {Definition.Name}Model from, {Definition.Name}ViewModel to)");
	WriteLine("	{");
	WriteLine("		MapPartialToViewModel(map, from, to);");
	WriteLine("");
	foreach(var entry in Entries.Where(e => e.IsArray))
	{
		WriteLine($"		to.{entry.Field} = [.. map.From<{entry.EntryType}Model>(from.{entry.Field}).To<{entry.EntryType}ViewModel>()];");
		WriteLine("");
		WriteLine($"		foreach (var e in to.{entry.Field}) e.ParentId = from.Id;");
	}
	WriteLine("	}");
}
else
{
	WriteLine($"	public static partial void MapToViewModel(IMap map, {Definition.Name}Updated from, {Definition.Name}ViewModel to);");
	WriteLine("");
	WriteLine($"	public static partial void MapToModel(IMap map, {Definition.Name}ViewModel from, {Definition.Name}Model to);");
	WriteLine("");
	WriteLine($"	public static partial void MapToViewModel(IMap map, {Definition.Name}Model from, {Definition.Name}ViewModel to);");
}
#>
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>