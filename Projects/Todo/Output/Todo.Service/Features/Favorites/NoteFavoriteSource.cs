using Najlot.Map;
using Todo.Contracts.Shared;
using Todo.Service.Features.Auth;
using Todo.Service.Features.Notes;

namespace Todo.Service.Features.Favorites;

public sealed class NoteFavoriteSource(
	INoteRepository noteRepository,
	IMap map,
	IPermissionService permissionService) : IFavoriteSource
{
	public ItemType Type => ItemType.Note;

	public Task<FavoriteModel?> CreateFavoriteAsync(Guid userId, Guid itemId)
	{
		var item = permissionService
			.ApplyReadFilter(noteRepository.GetAllQueryable())
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
