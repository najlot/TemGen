using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.Notes;


namespace Todo.Service.Features.Notes;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(Note))]
[JsonSerializable(typeof(List<NoteListItem>))]
[JsonSerializable(typeof(NoteListItem[]))]
[JsonSerializable(typeof(CreateNote))]
[JsonSerializable(typeof(UpdateNote))]
[JsonSerializable(typeof(NoteModel))]
[JsonSerializable(typeof(NoteCreated))]
[JsonSerializable(typeof(NoteUpdated))]
[JsonSerializable(typeof(NoteDeleted))]
public partial class NoteSerializerContext : JsonSerializerContext
{
}
