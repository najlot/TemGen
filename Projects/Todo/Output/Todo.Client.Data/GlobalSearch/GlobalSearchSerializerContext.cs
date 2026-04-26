using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.GlobalSearch;

namespace Todo.Client.Data.GlobalSearch;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(GlobalSearchItem[]))]
public partial class GlobalSearchSerializerContext : JsonSerializerContext
{
}
