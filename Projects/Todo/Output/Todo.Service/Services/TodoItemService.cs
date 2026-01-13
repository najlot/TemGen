using Cosei.Service.Base;
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

public class TodoItemService
{
	private readonly ITodoItemRepository _todoItemRepository;
	private readonly IUserRepository _userRepository;
	private readonly IPublisher _publisher;
	private readonly IMap _map;

	public TodoItemService(
		ITodoItemRepository todoItemRepository,
			IUserRepository userRepository,
		IPublisher publisher,
		IMap map)
	{
		_todoItemRepository = todoItemRepository;
			_userRepository = userRepository;
		_publisher = publisher;
		_map = map;
	}

	public async Task CreateTodoItem(CreateTodoItem command, Guid userId)
	{
		var item = _map.From(command).To<TodoItemModel>();

		await _todoItemRepository.Insert(item).ConfigureAwait(false);

		var message = _map.From(item).To<TodoItemCreated>();
		await _publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task UpdateTodoItem(UpdateTodoItem command, Guid userId)
	{
		var item = await _todoItemRepository.Get(command.Id).ConfigureAwait(false);

		if (item == null)
		{
			throw new InvalidOperationException("TodoItem not found!");
		}

		_map.From(command).To(item);

		await _todoItemRepository.Update(item).ConfigureAwait(false);

		var message = _map.From(item).To<TodoItemUpdated>();
		await _publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task DeleteTodoItem(Guid id, Guid userId)
	{
		await _todoItemRepository.Delete(id).ConfigureAwait(false);

		var message = new TodoItemDeleted(id);
		await _publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task<TodoItem?> GetItemAsync(Guid id, Guid userId)
	{
		var item = await _todoItemRepository.Get(id).ConfigureAwait(false);
		return _map.FromNullable(item)?.To<TodoItem>();
	}

	public IAsyncEnumerable<TodoItemListItem> GetItemsForUserAsync(TodoItemFilter filter, Guid userId)
	{
		var enumerable = _todoItemRepository.GetAll();

		if (!string.IsNullOrEmpty(filter.Title))
			enumerable = enumerable.Where(e => e.Title.Contains(filter.Title));
		if (!string.IsNullOrEmpty(filter.Content))
			enumerable = enumerable.Where(e => e.Content.Contains(filter.Content));
		if (filter.CreatedAtFrom != null)
			enumerable = enumerable.Where(e => e.CreatedAt >= filter.CreatedAtFrom);
		if (filter.CreatedAtTo != null)
			enumerable = enumerable.Where(e => e.CreatedAt <= filter.CreatedAtTo);
		if (!string.IsNullOrEmpty(filter.CreatedBy))
			enumerable = enumerable.Where(e => e.CreatedBy.Contains(filter.CreatedBy));
		if (filter.AssignedToId != null)
			enumerable = enumerable.Where(e => e.AssignedToId == filter.AssignedToId);
		if (filter.Status != null)
			enumerable = enumerable.Where(e => e.Status == filter.Status);
		if (filter.ChangedAtFrom != null)
			enumerable = enumerable.Where(e => e.ChangedAt >= filter.ChangedAtFrom);
		if (filter.ChangedAtTo != null)
			enumerable = enumerable.Where(e => e.ChangedAt <= filter.ChangedAtTo);
		if (!string.IsNullOrEmpty(filter.ChangedBy))
			enumerable = enumerable.Where(e => e.ChangedBy.Contains(filter.ChangedBy));
		if (!string.IsNullOrEmpty(filter.Priority))
			enumerable = enumerable.Where(e => e.Priority.Contains(filter.Priority));

		return _map.From(enumerable).To<TodoItemListItem>();
	}

	public IAsyncEnumerable<TodoItemListItem> GetItemsForUserAsync(Guid userId)
	{
		var enumerable = _todoItemRepository.GetAll();
		return _map.From(enumerable).To<TodoItemListItem>();
	}
}