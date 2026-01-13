using Najlot.Map;
using Todo.Client.Data.Models;
using Todo.Contracts;
using Todo.Contracts.Commands;
using Todo.Contracts.Events;
using Todo.Contracts.ListItems;

namespace Todo.Client.Data.Mappings;

internal sealed class NoteMappings
{
	public CreateNote MapToCreate(IMap map, NoteModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.Color);

	public NoteCreated MapToCreated(IMap map, NoteModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.Color);

	public UpdateNote MapToUpdate(IMap map, NoteModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.Color);

	public NoteUpdated MapToUpdated(IMap map, NoteModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.Color);

	public void MapToModel(IMap map, NoteCreated from, NoteListItemModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
	}

	public void MapToModel(IMap map, NoteUpdated from, NoteListItemModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
	}

	public void MapToModel(IMap map, NoteListItem from, NoteListItemModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
	}

	public void MapModelToModel(IMap map, NoteModel from, NoteListItemModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
	}

	public void MapToModel(IMap map, Note from, NoteModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
		to.Color = from.Color;
	}

	public void MapToModel(IMap map, NoteUpdated from, NoteModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
		to.Color = from.Color;
	}
}