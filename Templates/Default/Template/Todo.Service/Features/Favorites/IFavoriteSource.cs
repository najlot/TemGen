using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.Service.Features.Favorites;

public interface IFavoriteSource
{
	ItemType Type { get; }

	Task<FavoriteModel?> CreateFavoriteAsync(Guid userId, Guid itemId);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>