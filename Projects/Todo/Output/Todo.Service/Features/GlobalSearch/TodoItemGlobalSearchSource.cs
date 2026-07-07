using Najlot.Map;
using System.Collections.Generic;
using System.Threading;
using Todo.Contracts.GlobalSearch;
using Todo.Service.Features.Auth;
using Todo.Service.Features.TodoItems;

namespace Todo.Service.Features.GlobalSearch;

public class TodoItemGlobalSearchSource(
	ITodoItemRepository todoItemRepository,
	IMap map,
	IPermissionService permissionService) : IGlobalSearchSource
{
	public IAsyncEnumerable<GlobalSearchItem> SearchAsync(string text, CancellationToken cancellationToken = default)
	{
		var normalizedText = text.ToLower();
		var query = permissionService
			.ApplyReadFilter(todoItemRepository.GetAllQueryable())
			.Where(item => item.DeletedAt == null)
			.Where(item =>
				item.Title.ToLower().Contains(normalizedText) ||
				item.Content.ToLower().Contains(normalizedText));

		cancellationToken.ThrowIfCancellationRequested();
		return map.From(query).To<GlobalSearchItem>().ToAsyncEnumerable();
	}
}