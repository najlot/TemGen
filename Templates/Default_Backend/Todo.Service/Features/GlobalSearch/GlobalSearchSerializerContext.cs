using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using <# Project.Namespace#>.Contracts.GlobalSearch;

namespace <# Project.Namespace#>.Service.Features.GlobalSearch;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(GlobalSearchItem[]))]
[JsonSerializable(typeof(List<GlobalSearchItem>))]
public partial class GlobalSearchSerializerContext : JsonSerializerContext
{
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>