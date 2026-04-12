using System;
using System.Threading.Tasks;
using Todo.Contracts.TodoItems;

namespace Todo.Client.Data.TodoItems;

public interface ITodoItemRepository
{
	Task<TodoItemListItemModel[]> GetItemsAsync();

	Task<TodoItemListItemModel[]> GetItemsAsync(TodoItemFilter filter);

	Task<TodoItemModel> GetItemAsync(Guid id);

	Task AddItemAsync(TodoItemModel item);

	Task UpdateItemAsync(TodoItemModel item);

	Task DeleteItemAsync(Guid id);
}