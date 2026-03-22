using LiteDB;
using Najlot.Map;
using Todo.Contracts;
using Todo.Contracts.Events;
using Todo.Service.Model;
using Todo.Service.Repository;

namespace Todo.Service.Services.Trash;

public class TodoItemTrashSource(
	ITodoItemRepository todoItemRepository,
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

		item.DeletedAt = null;
		await todoItemRepository.Update(item).ConfigureAwait(false);

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
			await todoItemRepository.Delete(item.Id).ConfigureAwait(false);
			await publisher.PublishAsync(new TrashItemDeleted(item.Id, ItemType.TodoItem)).ConfigureAwait(false);
		}
	}
}
