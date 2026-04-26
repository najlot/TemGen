using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.History;

namespace Todo.Service.Features.History;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(HistoryChange[]))]
[JsonSerializable(typeof(HistoryEntry[]))]
[JsonSerializable(typeof(HistoryModel))]
public partial class HistorySerializerContext : JsonSerializerContext
{
}
