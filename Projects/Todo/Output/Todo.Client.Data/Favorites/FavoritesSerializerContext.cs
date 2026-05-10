using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.Favorites;

namespace Todo.Client.Data.Favorites;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(Favorite))]
[JsonSerializable(typeof(Favorite[]))]
[JsonSerializable(typeof(CreateFavorite))]
[JsonSerializable(typeof(FavoriteCreated))]
[JsonSerializable(typeof(FavoriteUpdated))]
[JsonSerializable(typeof(FavoriteDeleted))]
public partial class FavoritesSerializerContext : JsonSerializerContext
{
}
