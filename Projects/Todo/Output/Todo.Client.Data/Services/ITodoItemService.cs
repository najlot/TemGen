using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Contracts.Filters;

namespace Todo.Client.Data.Services;

public interface ITodoItemService : IDisposable
{
	TodoItemModel CreateTodoItem();
	Task AddItemAsync(TodoItemModel item);
	Task<IEnumerable<TodoItemListItemModel>> GetItemsAsync();
	Task<IEnumerable<TodoItemListItemModel>> GetItemsAsync(TodoItemFilter filter);
	Task<TodoItemModel> GetItemAsync(Guid id);
	Task UpdateItemAsync(TodoItemModel item);
	Task DeleteItemAsync(Guid id);
}