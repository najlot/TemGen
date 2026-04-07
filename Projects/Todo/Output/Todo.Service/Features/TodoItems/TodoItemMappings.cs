using Najlot.Map;
using Najlot.Map.Attributes;
using System.Linq.Expressions;
using Todo.Contracts;
using Todo.Contracts.Commands;
using Todo.Contracts.Events;
using Todo.Contracts.ListItems;

namespace Todo.Service.Features.TodoItems;

[Mapping]
internal partial class TodoItemMappings
{
	public static partial void MapToCreated(IMap map, TodoItemModel from, TodoItemCreated to);

	public static partial void MapToUpdated(IMap map, TodoItemModel from, TodoItemUpdated to);

	[MapIgnoreProperty(nameof(to.DeletedAt))]
	[MapIgnoreProperty(nameof(to.Checklist))]
	[PostMap(nameof(PostMapToModel))]
	public static partial void MapToModel(IMap map, CreateTodoItem from, TodoItemModel to);
	private static void PostMapToModel(IMap map, CreateTodoItem from, TodoItemModel to)
	{
		to.Checklist = map.From<ChecklistTask>(from.Checklist).ToList(to.Checklist);
	}

	[MapIgnoreProperty(nameof(to.DeletedAt))]
	[MapIgnoreProperty(nameof(to.Checklist))]
	[PostMap(nameof(PostMapToModel))]
	public static partial void MapToModel(IMap map, UpdateTodoItem from, TodoItemModel to);

	private static void PostMapToModel(IMap map, UpdateTodoItem from, TodoItemModel to)
	{
		to.Checklist = map.From<ChecklistTask>(from.Checklist).ToList(to.Checklist);
	}

	public static partial void MapToModel(IMap map, TodoItemModel from, TodoItem to);

	public static Expression<Func<TodoItemModel, TodoItemListItem>> GetListItemExpression()
	{
		return from => new TodoItemListItem
		{
			Id = from.Id,
			Title = from.Title,
			Content = from.Content
		};
	}

	public static partial void MapToModel(IMap map, TodoItemModel from, TodoItemListItem to);
}