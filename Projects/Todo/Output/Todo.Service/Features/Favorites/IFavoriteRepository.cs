using Todo.Service.Infrastructure.Persistence;

namespace Todo.Service.Features.Favorites;

public interface IFavoriteRepository : IEntityRepository<FavoriteModel>
{
}
