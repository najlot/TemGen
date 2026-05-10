using System;
using System.Threading.Tasks;
using <# Project.Namespace#>.Contracts.Favorites;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.Client.Data.Favorites;

public interface IFavoriteService
{
	event AsyncEventHandler<FavoriteCreated>? ItemCreated;
	event AsyncEventHandler<FavoriteUpdated>? ItemUpdated;
	event AsyncEventHandler<FavoriteDeleted>? ItemDeleted;

	Task StartEventListener();
	Task<Favorite[]> GetItemsAsync(ItemType targetType);
	Task AddItemAsync(ItemType targetType, Guid itemId);
	Task DeleteItemAsync(ItemType targetType, Guid itemId);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>