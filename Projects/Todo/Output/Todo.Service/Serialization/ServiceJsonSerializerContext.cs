using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.Auth;
using Todo.Contracts.GlobalSearch;
using Todo.Contracts.History;
using Todo.Contracts.Trash;
using Todo.Service.Features.History;
using Todo.Service.Shared.Configuration;
using Todo.Contracts.Notes;
using Todo.Contracts.TodoItems;
using Todo.Service.Features.Notes;
using Todo.Service.Features.TodoItems;
using Todo.Contracts.Users;
using Todo.Service.Features.Users;

namespace Todo.Service.Serialization;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(AuthRequest))]
[JsonSerializable(typeof(GlobalSearchItem[]))]
[JsonSerializable(typeof(List<GlobalSearchItem>))]
[JsonSerializable(typeof(HistoryChange[]))]
[JsonSerializable(typeof(HistoryEntry[]))]
[JsonSerializable(typeof(TrashItem[]))]
[JsonSerializable(typeof(List<TrashItem>))]
[JsonSerializable(typeof(TrashItemCreated))]
[JsonSerializable(typeof(TrashItemUpdated))]
[JsonSerializable(typeof(TrashItemDeleted))]
[JsonSerializable(typeof(HistoryModel))]
[JsonSerializable(typeof(StorageConfiguration))]
[JsonSerializable(typeof(BackupConfiguration))]
[JsonSerializable(typeof(FileConfiguration))]
[JsonSerializable(typeof(LiteDbConfiguration))]
[JsonSerializable(typeof(MongoDbConfiguration))]
[JsonSerializable(typeof(MySqlConfiguration))]
[JsonSerializable(typeof(ServiceConfiguration))]
[JsonSerializable(typeof(ChecklistTask))]
[JsonSerializable(typeof(Note))]
[JsonSerializable(typeof(TodoItem))]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(List<NoteListItem>))]
[JsonSerializable(typeof(NoteListItem[]))]
[JsonSerializable(typeof(CreateNote))]
[JsonSerializable(typeof(UpdateNote))]
[JsonSerializable(typeof(NoteModel))]
[JsonSerializable(typeof(NoteCreated))]
[JsonSerializable(typeof(NoteUpdated))]
[JsonSerializable(typeof(NoteDeleted))]
[JsonSerializable(typeof(List<TodoItemListItem>))]
[JsonSerializable(typeof(TodoItemListItem[]))]
[JsonSerializable(typeof(CreateTodoItem))]
[JsonSerializable(typeof(UpdateTodoItem))]
[JsonSerializable(typeof(TodoItemModel))]
[JsonSerializable(typeof(TodoItemCreated))]
[JsonSerializable(typeof(TodoItemUpdated))]
[JsonSerializable(typeof(TodoItemDeleted))]
[JsonSerializable(typeof(List<UserListItem>))]
[JsonSerializable(typeof(UserListItem[]))]
[JsonSerializable(typeof(CreateUser))]
[JsonSerializable(typeof(UpdateUser))]
[JsonSerializable(typeof(UserModel))]
[JsonSerializable(typeof(UserCreated))]
[JsonSerializable(typeof(UserUpdated))]
[JsonSerializable(typeof(UserDeleted))]
[JsonSerializable(typeof(NoteFilter))]
[JsonSerializable(typeof(TodoItemFilter))]
public partial class ServiceJsonSerializerContext : JsonSerializerContext
{
}
