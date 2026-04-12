using Najlot.Map;
using Najlot.Map.Attributes;
using System.Linq.Expressions;
using Todo.Contracts.TodoItems;
using Todo.Contracts.Trash;
using Todo.Contracts.Shared;
using Todo.Service.Features.TodoItems;

namespace Todo.Service.Features.Trash;

[Mapping]
internal partial class TodoItemTrashMappings
{
	public static TrashItemCreated MapToTrashItemCreated(IMap map, TodoItemModel from) =>
		new()
		{
			Id = from.Id,
			Type = ItemType.TodoItem,
			Title = from.Title,
			Content = from.Content,
			DeletedAt = from.DeletedAt,
		};

	public static TrashItemUpdated MapToTrashItemUpdated(IMap map, TodoItemModel from) =>
		new()
		{
			Id = from.Id,
			Type = ItemType.TodoItem,
			Title = from.Title,
			Content = from.Content,
			DeletedAt = from.DeletedAt,
		};

	public static Expression<Func<TodoItemModel, TrashItem>> GetTrashItemExpression()
	{
		return from => new TrashItem
		{
			Id = from.Id,
			Type = ItemType.TodoItem,
			Title = from.Title,
			Content = from.Content,
			DeletedAt = from.DeletedAt,
		};
	}

	public static void MapToTrashItem(IMap map, TodoItemModel from, TrashItem to)
	{
		to.Id = from.Id;
		to.Type = ItemType.TodoItem;
		to.Title = from.Title;
		to.Content = from.Content;
		to.DeletedAt = from.DeletedAt;
	}
}
