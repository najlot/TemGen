using System.Text.Json;
using System.Text.Json.Serialization;
using <# Project.Namespace#>.Contracts.Trash;

namespace <# Project.Namespace#>.Client.Data.Trash;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(TrashItem[]))]
public partial class TrashSerializerContext : JsonSerializerContext
{
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>