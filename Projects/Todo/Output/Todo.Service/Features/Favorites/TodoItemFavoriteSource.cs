using Najlot.Map;
using Todo.Contracts.Shared;
using Todo.Service.Features.Auth;
using Todo.Service.Features.TodoItems;

namespace Todo.Service.Features.Favorites;

public sealed class TodoItemFavoriteSource(
	ITodoItemRepository todoItemRepository,
	IMap map,
	IPermissionQueryFilter permissionQueryFilter) : IFavoriteSource
{
	public ItemType Type => ItemType.TodoItem;

	public Task<FavoriteModel?> CreateFavoriteAsync(Guid userId, Guid itemId)
	{
		var item = permissionQueryFilter
			.ApplyReadFilter(todoItemRepository.GetAllQueryable())
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
