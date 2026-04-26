using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.TodoItems;


namespace Todo.Service.Features.TodoItems;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(TodoItem))]
[JsonSerializable(typeof(List<TodoItemListItem>))]
[JsonSerializable(typeof(TodoItemListItem[]))]
[JsonSerializable(typeof(CreateTodoItem))]
[JsonSerializable(typeof(UpdateTodoItem))]
[JsonSerializable(typeof(TodoItemModel))]
[JsonSerializable(typeof(TodoItemCreated))]
[JsonSerializable(typeof(TodoItemUpdated))]
[JsonSerializable(typeof(TodoItemDeleted))]
public partial class TodoItemSerializerContext : JsonSerializerContext
{
}
