using System.Text.Json;
using System.Text.Json.Serialization;
using <# Project.Namespace#>.Contracts.Filters;

namespace <# Project.Namespace#>.Client.Data.Filters;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(EntityFilter))]
[JsonSerializable(typeof(Filter))]
[JsonSerializable(typeof(Filter[]))]
[JsonSerializable(typeof(CreateFilter))]
[JsonSerializable(typeof(UpdateFilter))]
public partial class FiltersSerializerContext : JsonSerializerContext
{
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>