using System.Text.Json;
using System.Text.Json.Serialization;
using <# Project.Namespace#>.Contracts.Filters;

namespace <# Project.Namespace#>.Service.Features.Filters;

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
<#cs SetOutputPathAndSkipOtherDefinitions()#>