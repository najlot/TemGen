using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.History;

namespace Todo.Client.Data.History;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(HistoryEntry[]))]
public partial class HistorySerializerContext : JsonSerializerContext
{
}
