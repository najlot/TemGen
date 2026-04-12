using Najlot.Map;
using Najlot.Map.Attributes;
using System.Linq.Expressions;
using Todo.Contracts.Notes;
using Todo.Contracts.Trash;
using Todo.Contracts.Shared;
using Todo.Service.Features.Notes;

namespace Todo.Service.Features.Trash;

[Mapping]
internal partial class NoteTrashMappings
{
	public static TrashItemCreated MapToTrashItemCreated(IMap map, NoteModel from) =>
		new()
		{
			Id = from.Id,
			Type = ItemType.Note,
			Title = from.Title,
			Content = from.Content,
			DeletedAt = from.DeletedAt,
		};

	public static TrashItemUpdated MapToTrashItemUpdated(IMap map, NoteModel from) =>
		new()
		{
			Id = from.Id,
			Type = ItemType.Note,
			Title = from.Title,
			Content = from.Content,
			DeletedAt = from.DeletedAt,
		};

	public static Expression<Func<NoteModel, TrashItem>> GetTrashItemExpression()
	{
		return from => new TrashItem
		{
			Id = from.Id,
			Type = ItemType.Note,
			Title = from.Title,
			Content = from.Content,
			DeletedAt = from.DeletedAt,
		};
	}

	public static void MapToTrashItem(IMap map, NoteModel from, TrashItem to)
	{
		to.Id = from.Id;
		to.Type = ItemType.Note;
		to.Title = from.Title;
		to.Content = from.Content;
		to.DeletedAt = from.DeletedAt;
	}
}
