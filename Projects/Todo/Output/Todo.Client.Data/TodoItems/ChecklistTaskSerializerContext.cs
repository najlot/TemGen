using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.TodoItems;

namespace Todo.Client.Data.TodoItems;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(ChecklistTask))]
public partial class ChecklistTaskSerializerContext : JsonSerializerContext
{
}
