using Najlot.Map;
using Todo.Contracts.GlobalSearch;
using Todo.Service.Features.Auth;
using Todo.Service.Features.TodoItems;

namespace Todo.Service.Features.GlobalSearch;

public class TodoItemGlobalSearchSource(
	ITodoItemRepository todoItemRepository,
	IMap map,
	IPermissionQueryFilter permissionQueryFilter) : IGlobalSearchSource
{
	public IAsyncEnumerable<GlobalSearchItem> SearchAsync(string text)
	{
		var normalizedText = text.ToLower();
		var query = permissionQueryFilter
			.ApplyReadFilter(todoItemRepository.GetAllQueryable())
			.Where(item => item.DeletedAt == null)
			.Where(item =>
				item.Title.ToLower().Contains(normalizedText) ||
				item.Content.ToLower().Contains(normalizedText));

		return map.From(query).To<GlobalSearchItem>().ToAsyncEnumerable();
	}
}