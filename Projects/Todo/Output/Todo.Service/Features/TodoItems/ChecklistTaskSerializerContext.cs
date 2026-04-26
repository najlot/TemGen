using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.TodoItems;


namespace Todo.Service.Features.TodoItems;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(ChecklistTask))]
public partial class ChecklistTaskSerializerContext : JsonSerializerContext
{
}
