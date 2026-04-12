using System;
using System.Threading.Tasks;
using Todo.Contracts.TodoItems;

namespace Todo.Client.Data.TodoItems;

public interface ITodoItemService
{
	event AsyncEventHandler<TodoItemCreated>? ItemCreated;
	event AsyncEventHandler<TodoItemUpdated>? ItemUpdated;
	event AsyncEventHandler<TodoItemDeleted>? ItemDeleted;

	Task StartEventListener();

	TodoItemModel CreateTodoItem();
	Task AddItemAsync(TodoItemModel item);
	Task<TodoItemListItemModel[]> GetItemsAsync();
	Task<TodoItemListItemModel[]> GetItemsAsync(TodoItemFilter filter);
	Task<TodoItemModel> GetItemAsync(Guid id);
	Task UpdateItemAsync(TodoItemModel item);
	Task DeleteItemAsync(Guid id);
}