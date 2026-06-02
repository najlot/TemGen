using Najlot.Map;
using <# Project.Namespace#>.Contracts.Shared;
using <# Project.Namespace#>.Service.Features.Auth;
using <# Project.Namespace#>.Service.Features.<# Definition.Name#>s;

namespace <# Project.Namespace#>.Service.Features.Favorites;

public sealed class <# Definition.Name#>FavoriteSource(
	I<# Definition.Name#>Repository <# Definition.NameLow#>Repository,
	IMap map,
	IPermissionService permissionService) : IFavoriteSource
{
	public ItemType Type => ItemType.<# Definition.Name#>;

	public Task<FavoriteModel?> CreateFavoriteAsync(Guid userId, Guid itemId)
	{
		var item = permissionService
			.ApplyReadFilter(<# Definition.NameLow#>Repository.GetAllQueryable())
			.Where(item => item.DeletedAt == null && item.Id == itemId)
			.FirstOrDefault();

		if (item is null)
		{
			return Task.FromResult<FavoriteModel?>(null);
		}

		var favorite = map.From(item).To<FavoriteModel>();
		favorite.Id = Guid.NewGuid();
		favorite.UserId = userId;
		return Task.FromResult<FavoriteModel?>(favorite);
	}
}
<#cs SetOutputPath(Definition.IsOwnedType
	|| Definition.IsEnumeration
	|| Definition.IsArray
	|| Definition.NameLow == "user")#>