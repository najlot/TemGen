using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using <# Project.Namespace#>.Contracts.Trash;

namespace <# Project.Namespace#>.Service.Features.Trash;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(TrashItem[]))]
[JsonSerializable(typeof(List<TrashItem>))]
[JsonSerializable(typeof(TrashItemCreated))]
[JsonSerializable(typeof(TrashItemUpdated))]
[JsonSerializable(typeof(TrashItemDeleted))]
public partial class TrashSerializerContext : JsonSerializerContext
{
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>