using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.Notes;

namespace Todo.Client.Data.Notes;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(Note))]
[JsonSerializable(typeof(NoteListItem[]))]
[JsonSerializable(typeof(CreateNote))]
[JsonSerializable(typeof(NoteCreated))]
[JsonSerializable(typeof(UpdateNote))]
[JsonSerializable(typeof(NoteUpdated))]
[JsonSerializable(typeof(NoteDeleted))]
public partial class NoteSerializerContext : JsonSerializerContext
{
}
