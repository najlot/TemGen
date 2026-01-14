using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.Models;
using Todo.Contracts;
using Todo.Contracts.Commands;
using Todo.Contracts.Events;
using Todo.Contracts.ListItems;

namespace Todo.Client.Data.Mappings;

[Mapping]
internal sealed partial class NoteMappings
{
	public static CreateNote MapToCreate(IMap map, NoteModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.Color);

	public static NoteCreated MapToCreated(IMap map, NoteModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.Color);

	public static UpdateNote MapToUpdate(IMap map, NoteModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.Color);

	public static NoteUpdated MapToUpdated(IMap map, NoteModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.Color);

	public static partial void MapToModel(IMap map, NoteCreated from, NoteListItemModel to);

	public static partial void MapToModel(IMap map, NoteUpdated from, NoteListItemModel to);

	public static partial void MapToModel(IMap map, NoteListItem from, NoteListItemModel to);

	public static partial void MapModelToModel(IMap map, NoteModel from, NoteListItemModel to);

	public static partial void MapToModel(IMap map, Note from, NoteModel to);

	public static partial void MapToModel(IMap map, NoteUpdated from, NoteModel to);
}