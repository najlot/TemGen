using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.Filters;

namespace Todo.Service.Features.Filters;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(EntityFilter))]
[JsonSerializable(typeof(Filter))]
[JsonSerializable(typeof(Filter[]))]
[JsonSerializable(typeof(CreateFilter))]
[JsonSerializable(typeof(UpdateFilter))]
[JsonSerializable(typeof(FilterModel))]
public partial class FiltersSerializerContext : JsonSerializerContext
{
}
