using Najlot.Map.Attributes;
using System.Linq.Expressions;
using <# Project.Namespace#>.Contracts.GlobalSearch;
using <# Project.Namespace#>.Contracts.Trash;
using <# Project.Namespace#>.Contracts.Shared;
using <# Project.Namespace#>.Service.Features.<# Definition.Name#>s;

namespace <# Project.Namespace#>.Service.Features.GlobalSearch;

[Mapping]
internal partial class <# Definition.Name#>GlobalSearchMappings
{
	public static Expression<Func<<# Definition.Name#>Model, GlobalSearchItem>> Get<# Definition.Name#>Expression()
	{
		return from => new GlobalSearchItem
		{
			Id = from.Id,
			Type = ItemType.<# Definition.Name#>,
<#cs var searchableEntries = Entries.Where(e => e.EntryType.Equals("string", StringComparison.OrdinalIgnoreCase)).Take(2).ToList();
if (searchableEntries.Count > 0) WriteLine($"\t\t\tTitle = from.{searchableEntries[0].Field},"); else WriteLine("\t\t\tTitle = string.Empty,");
if (searchableEntries.Count > 1) WriteLine($"\t\t\tContent = from.{searchableEntries[1].Field},"); else WriteLine("\t\t\tContent = string.Empty,");
#>		};
	}
}<#cs SetOutputPath(Definition.IsOwnedType
	|| Definition.IsEnumeration
	|| Definition.IsArray
	|| Definition.NameLow == "user")#>
