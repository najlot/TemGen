using Najlot.Map;
using Todo.Contracts;
using Todo.Contracts.Commands;
using Todo.Contracts.Events;
using Todo.Contracts.ListItems;
using Todo.Contracts.Filters;
using Todo.Service.Features.Auth;
using Todo.Service.Features.History;
using Todo.Service.Shared.Realtime;
using Todo.Service.Shared.Results;

namespace Todo.Service.Features.TodoItems;

public class TodoItemService(
	ITodoItemRepository todoItemRepository,
	HistoryService historyService,
	IPublisher publisher,
	IMap map,
	IPermissionQueryFilter permissionQueryFilter)
{
	public async Task<Result> CreateTodoItem(CreateTodoItem command)
	{
		var item = new TodoItemModel();
		var snapshot = historyService.CreateSnapshot(item);
		map.From(command).To(item);

		await todoItemRepository.Insert(item).ConfigureAwait(false);
		await historyService.WriteChangesAsync(item.Id, snapshot).ConfigureAwait(false);

		var message = map.From(item).To<TodoItemCreated>();
		await publisher.PublishAsync(message).ConfigureAwait(false);
		return Result.Success();
	}

	public async Task<Result> UpdateTodoItem(UpdateTodoItem command)
	{
		var item = await todoItemRepository.Get(command.Id).ConfigureAwait(false);

		if (item == null)
		{
			return Result.NotFound("TodoItem not found!");
		}

		var snapshot = historyService.CreateSnapshot(item);
		map.From(command).To(item);

		await todoItemRepository.Update(item).ConfigureAwait(false);
		await historyService.WriteChangesAsync(item.Id, snapshot).ConfigureAwait(false);

		var message = map.From(item).To<TodoItemUpdated>();
		await publisher.PublishAsync(message).ConfigureAwait(false);

		if (item.DeletedAt != null)
		{
			var trashItemUpdated = map.From(item).To<TrashItemUpdated>();
			await publisher.PublishAsync(trashItemUpdated).ConfigureAwait(false);
		}

		return Result.Success();
	}

	public async Task<Result> DeleteTodoItem(Guid id)
	{
		var item = await todoItemRepository.Get(id).ConfigureAwait(false);

		if (item == null)
		{
			return Result.NotFound("TodoItem not found!");
		}

		if (item.DeletedAt == null)
		{
			var snapshot = historyService.CreateSnapshot(item);
			item.DeletedAt = DateTime.UtcNow;
			await todoItemRepository.Update(item).ConfigureAwait(false);
			await historyService.WriteChangesAsync(item.Id, snapshot).ConfigureAwait(false);

			var trashItemCreated = map.From(item).To<TrashItemCreated>();
			await publisher.PublishAsync(trashItemCreated).ConfigureAwait(false);

			var message = new TodoItemDeleted(id);
			await publisher.PublishAsync(message).ConfigureAwait(false);
		}
		else
		{
			await historyService.DeleteHistoryEntriesAsync(item.Id).ConfigureAwait(false);
			await todoItemRepository.Delete(id).ConfigureAwait(false);
			var trashItemDeleted = new TrashItemDeleted(item.Id, ItemType.TodoItem);
			await publisher.PublishAsync(trashItemDeleted).ConfigureAwait(false);
		}

		return Result.Success();
	}

	public async Task<Result<TodoItem>> GetItemAsync(Guid id)
	{
		var item = await todoItemRepository.Get(id).ConfigureAwait(false);

		if (item == null)
		{
			return Result<TodoItem>.NotFound("TodoItem not found!");
		}

		return Result<TodoItem>.Success(map.From(item).To<TodoItem>());
	}

	public IAsyncEnumerable<TodoItemListItem> GetItemsForUserAsync(TodoItemFilter filter)
	{
		var query = todoItemRepository.GetAllQueryable();

		query = query.Where(e => e.DeletedAt == null);
		query = permissionQueryFilter.ApplyReadFilter(query);

		if (!string.IsNullOrEmpty(filter.Title))
			query = query.Where(e => e.Title.Contains(filter.Title));
		if (!string.IsNullOrEmpty(filter.Content))
			query = query.Where(e => e.Content.Contains(filter.Content));
		if (filter.CreatedAtFrom != null)
			query = query.Where(e => e.CreatedAt >= filter.CreatedAtFrom);
		if (filter.CreatedAtTo != null)
			query = query.Where(e => e.CreatedAt <= filter.CreatedAtTo);
		if (!string.IsNullOrEmpty(filter.CreatedBy))
			query = query.Where(e => e.CreatedBy.Contains(filter.CreatedBy));
		if (filter.AssignedToId != null)
			query = query.Where(e => e.AssignedToId == filter.AssignedToId);
		if (filter.Status != null)
			query = query.Where(e => e.Status == filter.Status);
		if (filter.ChangedAtFrom != null)
			query = query.Where(e => e.ChangedAt >= filter.ChangedAtFrom);
		if (filter.ChangedAtTo != null)
			query = query.Where(e => e.ChangedAt <= filter.ChangedAtTo);
		if (!string.IsNullOrEmpty(filter.ChangedBy))
			query = query.Where(e => e.ChangedBy.Contains(filter.ChangedBy));
		if (!string.IsNullOrEmpty(filter.Priority))
			query = query.Where(e => e.Priority.Contains(filter.Priority));

		return map.From(query).To<TodoItemListItem>().ToAsyncEnumerable();
	}

	public IAsyncEnumerable<TodoItemListItem> GetItemsForUserAsync()
	{
		var query = todoItemRepository.GetAllQueryable();

		query = query.Where(e => e.DeletedAt == null);
		query = permissionQueryFilter.ApplyReadFilter(query);

		return map.From(query).To<TodoItemListItem>().ToAsyncEnumerable();
	}
}