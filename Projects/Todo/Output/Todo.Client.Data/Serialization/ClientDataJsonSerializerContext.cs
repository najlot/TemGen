using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.Auth;
using Todo.Contracts.GlobalSearch;
using Todo.Contracts.History;
using Todo.Contracts.Trash;
using Todo.Contracts.Notes;
using Todo.Contracts.TodoItems;
using Todo.Contracts.Users;

namespace Todo.Client.Data.Serialization;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(AuthRequest))]
[JsonSerializable(typeof(GlobalSearchItem[]))]
[JsonSerializable(typeof(HistoryEntry[]))]
[JsonSerializable(typeof(TrashItem[]))]
[JsonSerializable(typeof(ChecklistTask))]
[JsonSerializable(typeof(Note))]
[JsonSerializable(typeof(TodoItem))]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(NoteListItem[]))]
[JsonSerializable(typeof(CreateNote))]
[JsonSerializable(typeof(NoteCreated))]
[JsonSerializable(typeof(UpdateNote))]
[JsonSerializable(typeof(NoteUpdated))]
[JsonSerializable(typeof(NoteDeleted))]
[JsonSerializable(typeof(TodoItemListItem[]))]
[JsonSerializable(typeof(CreateTodoItem))]
[JsonSerializable(typeof(TodoItemCreated))]
[JsonSerializable(typeof(UpdateTodoItem))]
[JsonSerializable(typeof(TodoItemUpdated))]
[JsonSerializable(typeof(TodoItemDeleted))]
[JsonSerializable(typeof(UserListItem[]))]
[JsonSerializable(typeof(CreateUser))]
[JsonSerializable(typeof(UserCreated))]
[JsonSerializable(typeof(UpdateUser))]
[JsonSerializable(typeof(UserUpdated))]
[JsonSerializable(typeof(UserDeleted))]
[JsonSerializable(typeof(NoteFilter))]
[JsonSerializable(typeof(TodoItemFilter))]
public partial class ClientDataJsonSerializerContext : JsonSerializerContext
{
}
