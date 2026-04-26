using System.Text.Json;
using System.Text.Json.Serialization;
using <# Project.Namespace#>.Contracts.History;

namespace <# Project.Namespace#>.Service.Features.History;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(HistoryChange[]))]
[JsonSerializable(typeof(HistoryEntry[]))]
[JsonSerializable(typeof(HistoryModel))]
public partial class HistorySerializerContext : JsonSerializerContext
{
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>