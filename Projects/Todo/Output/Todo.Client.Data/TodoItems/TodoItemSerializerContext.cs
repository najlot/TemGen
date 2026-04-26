using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.TodoItems;

namespace Todo.Client.Data.TodoItems;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(TodoItem))]
[JsonSerializable(typeof(TodoItemListItem[]))]
[JsonSerializable(typeof(CreateTodoItem))]
[JsonSerializable(typeof(TodoItemCreated))]
[JsonSerializable(typeof(UpdateTodoItem))]
[JsonSerializable(typeof(TodoItemUpdated))]
[JsonSerializable(typeof(TodoItemDeleted))]
public partial class TodoItemSerializerContext : JsonSerializerContext
{
}
