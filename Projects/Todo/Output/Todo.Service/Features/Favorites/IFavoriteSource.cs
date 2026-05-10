using Todo.Contracts.Shared;

namespace Todo.Service.Features.Favorites;

public interface IFavoriteSource
{
	ItemType Type { get; }

	Task<FavoriteModel?> CreateFavoriteAsync(Guid userId, Guid itemId);
}
