using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.Trash;

namespace Todo.Client.Data.Trash;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(TrashItem[]))]
public partial class TrashSerializerContext : JsonSerializerContext
{
}
