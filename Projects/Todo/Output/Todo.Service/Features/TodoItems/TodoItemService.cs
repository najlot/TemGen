using Najlot.Map;
using Todo.Contracts.Filters;
using Todo.Contracts.TodoItems;
using Todo.Contracts.Trash;
using Todo.Contracts.Shared;
using Todo.Service.Features.Auth;
using Todo.Service.Features.History;
using Todo.Service.Features.Filters;
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
	private static readonly HashSet<string> FilterableProperties = new(StringComparer.Ordinal)
	{
		nameof(TodoItemModel.Title),
		nameof(TodoItemModel.Content),
		nameof(TodoItemModel.CreatedAt),
		nameof(TodoItemModel.CreatedBy),
		nameof(TodoItemModel.AssignedToId),
		nameof(TodoItemModel.Status),
		nameof(TodoItemModel.ChangedAt),
		nameof(TodoItemModel.ChangedBy),
		nameof(TodoItemModel.Priority),
	};

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

	public IAsyncEnumerable<TodoItemListItem> GetItemsForUserAsync(EntityFilter filter)
	{
		if (filter.Conditions.Count == 0)
		{
			return GetItemsForUserAsync();
		}

		var query = todoItemRepository.GetAllQueryable();

		query = query.Where(e => e.DeletedAt == null);
		query = permissionQueryFilter.ApplyReadFilter(query);

		foreach (var condition in filter.Conditions)
		{
			if (!FilterableProperties.Contains(condition.Field))
			{
				continue;
			}

			query = query.ApplyFilter(condition.Field, condition);
		}

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