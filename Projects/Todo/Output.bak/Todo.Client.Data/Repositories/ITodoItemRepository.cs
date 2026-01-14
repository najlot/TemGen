using System;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Contracts.Filters;

namespace Todo.Client.Data.Repositories;

public interface ITodoItemRepository : IDisposable
{
	Task<TodoItemListItemModel[]> GetItemsAsync();

	Task<TodoItemListItemModel[]> GetItemsAsync(TodoItemFilter filter);

	Task<TodoItemModel> GetItemAsync(Guid id);

	Task AddItemAsync(TodoItemModel item);

	Task UpdateItemAsync(TodoItemModel item);

	Task DeleteItemAsync(Guid id);
}