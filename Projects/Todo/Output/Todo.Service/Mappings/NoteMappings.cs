using Najlot.Map;
using Todo.Contracts;
using Todo.Contracts.Commands;
using Todo.Contracts.Events;
using Todo.Contracts.ListItems;
using Todo.Service.Model;

namespace Todo.Service.Mappings;

internal class NoteMappings
{
	public NoteCreated MapToCreated(IMap map, NoteModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.Color);

	public NoteUpdated MapToUpdated(IMap map, NoteModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.Color);

	public void MapToModel(IMap map, CreateNote from, NoteModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
		to.Color = from.Color;
	}

	public void MapToModel(IMap map, UpdateNote from, NoteModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
		to.Color = from.Color;
	}

	public void MapToModel(IMap map, NoteModel from, Note to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
		to.Color = from.Color;
	}

	public void MapToModel(IMap map, NoteModel from, NoteListItem to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
	}
}