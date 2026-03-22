using Najlot.Map;
using Najlot.Map.Attributes;
using System.Linq.Expressions;
using Todo.Contracts;
using Todo.Contracts.Events;
using Todo.Service.Model;

namespace Todo.Service.Services.Trash;

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
