using Najlot.Map.Attributes;
using System.Linq.Expressions;
using Todo.Contracts;
using Todo.Service.Model;

namespace Todo.Service.Services.GlobalSearch;

[Mapping]
internal partial class TodoItemGlobalSearchMappings
{
	public static Expression<Func<TodoItemModel, GlobalSearchItem>> GetTodoItemExpression()
	{
		return from => new GlobalSearchItem
		{
			Id = from.Id,
			Type = ItemType.TodoItem,
			Title = from.Title,
			Content = from.Content,
		};
	}
}
