using Najlot.Map;
using Najlot.Map.Attributes;
using System.Linq.Expressions;
using <# Project.Namespace#>.Contracts.<# Definition.Name#>s;
using <# Project.Namespace#>.Contracts.Trash;
using <# Project.Namespace#>.Contracts.Shared;
using <# Project.Namespace#>.Service.Features.<# Definition.Name#>s;

namespace <# Project.Namespace#>.Service.Features.Trash;

[Mapping]
internal partial class <# Definition.Name#>TrashMappings
{
	public static TrashItemCreated MapToTrashItemCreated(IMap map, <# Definition.Name#>Model from) =>
		new()
		{
			Id = from.Id,
			Type = ItemType.<# Definition.Name#>,
<#cs var searchableEntries = Entries.Where(e => e.EntryType.Equals("string", StringComparison.OrdinalIgnoreCase)).Take(2).ToList();
if (searchableEntries.Count > 0) WriteLine($"\t\t\tTitle = from.{searchableEntries[0].Field},"); else WriteLine("\t\t\tTitle = string.Empty,");
if (searchableEntries.Count > 1) WriteLine($"\t\t\tContent = from.{searchableEntries[1].Field},"); else WriteLine("\t\t\tContent = string.Empty,");
#>			DeletedAt = from.DeletedAt,
		};

	public static TrashItemUpdated MapToTrashItemUpdated(IMap map, <# Definition.Name#>Model from) =>
		new()
		{
			Id = from.Id,
			Type = ItemType.<# Definition.Name#>,
<#cs var searchableEntries = Entries.Where(e => e.EntryType.Equals("string", StringComparison.OrdinalIgnoreCase)).Take(2).ToList();
if (searchableEntries.Count > 0) WriteLine($"\t\t\tTitle = from.{searchableEntries[0].Field},"); else WriteLine("\t\t\tTitle = string.Empty,");
if (searchableEntries.Count > 1) WriteLine($"\t\t\tContent = from.{searchableEntries[1].Field},"); else WriteLine("\t\t\tContent = string.Empty,");
#>			DeletedAt = from.DeletedAt,
		};

	public static Expression<Func<<# Definition.Name#>Model, TrashItem>> GetTrashItemExpression()
	{
		return from => new TrashItem
		{
			Id = from.Id,
			Type = ItemType.<# Definition.Name#>,
<#cs var searchableEntries = Entries.Where(e => e.EntryType.Equals("string", StringComparison.OrdinalIgnoreCase)).Take(2).ToList();
if (searchableEntries.Count > 0) WriteLine($"\t\t\tTitle = from.{searchableEntries[0].Field},"); else WriteLine("\t\t\tTitle = string.Empty,");
if (searchableEntries.Count > 1) WriteLine($"\t\t\tContent = from.{searchableEntries[1].Field},"); else WriteLine("\t\t\tContent = string.Empty,");
#>			DeletedAt = from.DeletedAt,
		};
	}

	public static void MapToTrashItem(IMap map, <# Definition.Name#>Model from, TrashItem to)
	{
		to.Id = from.Id;
		to.Type = ItemType.<# Definition.Name#>;
<#cs var searchableEntries = Entries.Where(e => e.EntryType.Equals("string", StringComparison.OrdinalIgnoreCase)).Take(2).ToList();
if (searchableEntries.Count > 0) WriteLine($"\t\tto.Title = from.{searchableEntries[0].Field};"); else WriteLine("\t\tto.Title = string.Empty;");
if (searchableEntries.Count > 1) WriteLine($"\t\tto.Content = from.{searchableEntries[1].Field};"); else WriteLine("\t\tto.Content = string.Empty;");
#>		to.DeletedAt = from.DeletedAt;
	}
}<#cs SetOutputPath(Definition.IsOwnedType
	|| Definition.IsEnumeration
	|| Definition.IsArray
	|| Definition.NameLow == "user")#>
