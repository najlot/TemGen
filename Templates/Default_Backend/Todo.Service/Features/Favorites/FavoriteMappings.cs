using Najlot.Map;
using Najlot.Map.Attributes;
using System;
using System.Linq.Expressions;
using <# Project.Namespace#>.Contracts.Favorites;

namespace <# Project.Namespace#>.Service.Features.Favorites;

[Mapping]
internal partial class FavoriteMappings
{
	public static partial void MapToCreated(IMap map, FavoriteModel from, FavoriteCreated to);

	public static partial void MapToUpdated(IMap map, FavoriteModel from, FavoriteUpdated to);

	public static Expression<Func<FavoriteModel, Favorite>> GetToFavoriteExpression()
	{
		return from => new Favorite
		{
			TargetType = from.TargetType,
			ItemId = from.ItemId,
			Title = from.Title,
			Content = from.Content,
		};
	}

	public static partial void MapToModel(IMap map, FavoriteModel from, Favorite to);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>