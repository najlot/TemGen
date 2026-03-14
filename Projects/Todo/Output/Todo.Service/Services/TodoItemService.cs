using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Najlot.Map;
using Todo.Contracts;
using Todo.Service.Model;
using Todo.Service.Repository;
using Todo.Contracts.Commands;
using Todo.Contracts.Events;
using Todo.Contracts.ListItems;
using Todo.Contracts.Filters;

namespace Todo.Service.Services;

public class TodoItemService(
	ITodoItemRepository todoItemRepository,
	IUserRepository userRepository,
	IPublisher publisher,
	IMap map)
{
	public async Task CreateTodoItem(CreateTodoItem command, Guid userId)
	{
		var item = map.From(command).To<TodoItemModel>();

		await todoItemRepository.Insert(item).ConfigureAwait(false);

		var message = map.From(item).To<TodoItemCreated>();
		await publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task UpdateTodoItem(UpdateTodoItem command, Guid userId)
	{
		var item = await todoItemRepository.Get(command.Id).ConfigureAwait(false);

		if (item == null)
		{
			throw new InvalidOperationException("TodoItem not found!");
		}

		map.From(command).To(item);

		await todoItemRepository.Update(item).ConfigureAwait(false);

		var message = map.From(item).To<TodoItemUpdated>();
		await publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task DeleteTodoItem(Guid id, Guid userId)
	{
		var item = await todoItemRepository.Get(id).ConfigureAwait(false);

		if (item == null)
		{
			throw new InvalidOperationException("TodoItem not found!");
		}

		if (item.DeletedAt == null)
		{
			item.DeletedAt = DateTime.UtcNow;
			await todoItemRepository.Update(item).ConfigureAwait(false);
		}
		else
		{
			await todoItemRepository.Delete(id).ConfigureAwait(false);
		}

		var message = new TodoItemDeleted(id);
		await publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task<TodoItem?> GetItemAsync(Guid id, Guid userId)
	{
		var item = await todoItemRepository.Get(id).ConfigureAwait(false);
		return map.FromNullable(item)?.To<TodoItem>();
	}

	public IAsyncEnumerable<TodoItemListItem> GetItemsForUserAsync(TodoItemFilter filter, Guid userId)
	{
		var query = todoItemRepository.GetAllQueryable();

		query = query.Where(e => e.DeletedAt == null);

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

	public IAsyncEnumerable<TodoItemListItem> GetItemsForUserAsync(Guid userId)
	{
		var query = todoItemRepository.GetAllQueryable();

		query = query.Where(e => e.DeletedAt == null);

		return map.From(query).To<TodoItemListItem>().ToAsyncEnumerable();
	}
}