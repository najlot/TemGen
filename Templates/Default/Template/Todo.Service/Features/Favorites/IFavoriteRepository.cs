using <# Project.Namespace#>.Service.Infrastructure.Persistence;

namespace <# Project.Namespace#>.Service.Features.Favorites;

public interface IFavoriteRepository : IEntityRepository<FavoriteModel>
{
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>