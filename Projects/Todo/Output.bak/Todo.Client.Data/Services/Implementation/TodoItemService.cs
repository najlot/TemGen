using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Client.Data.Repositories;
using Todo.Contracts.Filters;

namespace Todo.Client.Data.Services.Implementation;

public sealed class TodoItemService : ITodoItemService
{
	private readonly ITodoItemRepository _repository;

	public TodoItemService(ITodoItemRepository repository)
	{
		_repository = repository;
	}

	public TodoItemModel CreateTodoItem()
	{
		return new TodoItemModel()
		{
			Id = Guid.NewGuid(),
			Title = "",
			Content = "",
			CreatedBy = "",
			ChangedBy = "",
			Priority = "",
			Checklist = []
		};
	}

	public async Task AddItemAsync(TodoItemModel item)
	{
		await _repository.AddItemAsync(item);
	}

	public async Task DeleteItemAsync(Guid id)
	{
		await _repository.DeleteItemAsync(id);
	}

	public async Task<TodoItemModel> GetItemAsync(Guid id)
	{
		return await _repository.GetItemAsync(id);
	}

	public async Task<IEnumerable<TodoItemListItemModel>> GetItemsAsync()
	{
		return await _repository.GetItemsAsync();
	}

	public async Task<IEnumerable<TodoItemListItemModel>> GetItemsAsync(TodoItemFilter filter)
	{
		return await _repository.GetItemsAsync(filter);
	}

	public async Task UpdateItemAsync(TodoItemModel item)
	{
		await _repository.UpdateItemAsync(item);
	}

	public void Dispose() => _repository.Dispose();
}