using Najlot.Map;
using Najlot.Map.Attributes;
using System.Linq.Expressions;
using <# Project.Namespace#>.Contracts.Shared;
using <# Project.Namespace#>.Service.Features.<# Definition.Name#>s;

namespace <# Project.Namespace#>.Service.Features.Favorites;

[Mapping]
internal partial class <# Definition.Name#>FavoriteMappings
{
	public static Expression<Func<<# Definition.Name#>Model, FavoriteModel>> GetFavoriteModelExpression()
	{
		return from => new FavoriteModel
		{
			TargetType = ItemType.<# Definition.Name#>,
			ItemId = from.Id,
<#cs var searchableEntries = Entries.Where(e => e.EntryType.Equals("string", StringComparison.OrdinalIgnoreCase)).Take(2).ToList();
if (searchableEntries.Count > 0) WriteLine($"\t\t\tTitle = from.{searchableEntries[0].Field},"); else WriteLine("\t\t\tTitle = string.Empty,");
if (searchableEntries.Count > 1) WriteLine($"\t\t\tContent = from.{searchableEntries[1].Field},"); else WriteLine("\t\t\tContent = string.Empty,");
#>		};
	}

	[MapIgnoreProperty(nameof(to.Id))]
	[MapIgnoreProperty(nameof(to.UserId))]
	public static void MapToFavoriteModel(IMap map, <# Definition.Name#>Model from, FavoriteModel to)
	{
		to.TargetType = ItemType.<# Definition.Name#>;
		to.ItemId = from.Id;
<#cs var searchableEntries = Entries.Where(e => e.EntryType.Equals("string", StringComparison.OrdinalIgnoreCase)).Take(2).ToList();
if (searchableEntries.Count > 0) WriteLine($"\t\tto.Title = from.{searchableEntries[0].Field};"); else WriteLine("\t\tto.Title = string.Empty;");
if (searchableEntries.Count > 1) WriteLine($"\t\tto.Content = from.{searchableEntries[1].Field};"); else WriteLine("\t\tto.Content = string.Empty;");
#>	}
}
<#cs SetOutputPath(Definition.IsOwnedType
	|| Definition.IsEnumeration
	|| Definition.IsArray
	|| Definition.NameLow == "user")#>