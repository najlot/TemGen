using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.Filters;

namespace Todo.Client.Data.Filters;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(EntityFilter))]
[JsonSerializable(typeof(Filter))]
[JsonSerializable(typeof(Filter[]))]
[JsonSerializable(typeof(CreateFilter))]
[JsonSerializable(typeof(UpdateFilter))]
public partial class FiltersSerializerContext : JsonSerializerContext
{
}
