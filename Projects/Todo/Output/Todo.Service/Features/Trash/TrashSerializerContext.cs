using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.Trash;

namespace Todo.Service.Features.Trash;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(TrashItem[]))]
[JsonSerializable(typeof(List<TrashItem>))]
[JsonSerializable(typeof(TrashItemCreated))]
[JsonSerializable(typeof(TrashItemUpdated))]
[JsonSerializable(typeof(TrashItemDeleted))]
public partial class TrashSerializerContext : JsonSerializerContext
{
}
