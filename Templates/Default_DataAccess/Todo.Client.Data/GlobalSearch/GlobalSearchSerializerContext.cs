using System.Text.Json;
using System.Text.Json.Serialization;
using <# Project.Namespace#>.Contracts.GlobalSearch;

namespace <# Project.Namespace#>.Client.Data.GlobalSearch;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(GlobalSearchItem[]))]
public partial class GlobalSearchSerializerContext : JsonSerializerContext
{
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>