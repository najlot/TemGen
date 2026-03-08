using System;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Contracts.Events;
using Todo.Contracts.Filters;

namespace Todo.Client.Data.Services;

public interface ITodoItemService
{
	event AsyncEventHandler<TodoItemCreated>? OnItemCreated;
	event AsyncEventHandler<TodoItemUpdated>? OnItemUpdated;
	event AsyncEventHandler<TodoItemDeleted>? OnItemDeleted;

	Task StartEventListener();

	TodoItemModel CreateTodoItem();
	Task AddItemAsync(TodoItemModel item);
	Task<TodoItemListItemModel[]> GetItemsAsync();
	Task<TodoItemListItemModel[]> GetItemsAsync(TodoItemFilter filter);
	Task<TodoItemModel> GetItemAsync(Guid id);
	Task UpdateItemAsync(TodoItemModel item);
	Task DeleteItemAsync(Guid id);
}