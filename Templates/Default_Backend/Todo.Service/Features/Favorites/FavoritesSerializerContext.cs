using System.Text.Json;
using System.Text.Json.Serialization;
using <# Project.Namespace#>.Contracts.Favorites;

namespace <# Project.Namespace#>.Service.Features.Favorites;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(Favorite))]
[JsonSerializable(typeof(Favorite[]))]
[JsonSerializable(typeof(CreateFavorite))]
[JsonSerializable(typeof(FavoriteCreated))]
[JsonSerializable(typeof(FavoriteUpdated))]
[JsonSerializable(typeof(FavoriteDeleted))]
[JsonSerializable(typeof(FavoriteModel))]
public partial class FavoritesSerializerContext : JsonSerializerContext
{
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>