using System.Text.Json;
using System.Text.Json.Serialization;
using <# Project.Namespace#>.Contracts.History;

namespace <# Project.Namespace#>.Client.Data.History;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(HistoryEntry[]))]
public partial class HistorySerializerContext : JsonSerializerContext
{
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>