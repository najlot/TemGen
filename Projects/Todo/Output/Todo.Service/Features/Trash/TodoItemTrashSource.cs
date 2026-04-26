using Najlot.Map;
using Todo.Contracts.TodoItems;
using Todo.Contracts.Trash;
using Todo.Contracts.Shared;
using Todo.Service.Features.Auth;
using Todo.Service.Features.History;
using Todo.Service.Features.TodoItems;
using Todo.Service.Shared.Realtime;
using Todo.Service.Shared.Results;

namespace Todo.Service.Features.Trash;

public class TodoItemTrashSource(
	ITodoItemRepository todoItemRepository,
	HistoryService historyService,
	IPublisher publisher,
	IMap map,
	IPermissionQueryFilter permissionQueryFilter) : ITrashSource
{
	public ItemType Type => ItemType.TodoItem;

	public IAsyncEnumerable<TrashItem> GetItemsAsync()
	{
		var query = permissionQueryFilter
			.ApplyReadFilter(todoItemRepository.GetAllQueryable())
			.Where(item => item.DeletedAt != null)
			.OrderByDescending(item => item.DeletedAt);

		return map.From<TodoItemModel>(query).To<TrashItem>().ToAsyncEnumerable();
	}

	public async Task<Result> RestoreAsync(Guid id)
	{
		var item = await todoItemRepository.Get(id).ConfigureAwait(false);

		if (item == null || item.DeletedAt == null)
		{
			return Result.NotFound("Trash item not found!");
		}

		var snapshot = historyService.CreateSnapshot(item);
		item.DeletedAt = null;
		await todoItemRepository.Update(item).ConfigureAwait(false);
		await historyService.WriteChangesAsync(item.Id, snapshot).ConfigureAwait(false);

		await publisher.PublishAsync(new TrashItemDeleted(item.Id, ItemType.TodoItem)).ConfigureAwait(false);
		await publisher.PublishAsync(map.From(item).To<TodoItemCreated>()).ConfigureAwait(false);
		return Result.Success();
	}

	public async Task<Result> DeleteAsync(Guid id)
	{
		var item = await todoItemRepository.Get(id).ConfigureAwait(false);

		if (item == null || item.DeletedAt == null)
		{
			return Result.NotFound("Trash item not found!");
		}

		await historyService.DeleteHistoryEntriesAsync(item.Id).ConfigureAwait(false);
		await todoItemRepository.Delete(id).ConfigureAwait(false);
		await publisher.PublishAsync(new TrashItemDeleted(item.Id, ItemType.TodoItem)).ConfigureAwait(false);
		return Result.Success();
	}

	public async Task DeleteAllAsync()
	{
		var items = permissionQueryFilter
			.ApplyReadFilter(todoItemRepository.GetAllQueryable())
			.Where(item => item.DeletedAt != null)
			.ToList();

		foreach (var item in items)
		{
			await historyService.DeleteHistoryEntriesAsync(item.Id).ConfigureAwait(false);
			await todoItemRepository.Delete(item.Id).ConfigureAwait(false);
			await publisher.PublishAsync(new TrashItemDeleted(item.Id, ItemType.TodoItem)).ConfigureAwait(false);
		}
	}
}
