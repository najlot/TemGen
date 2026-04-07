using System;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Contracts;
using Todo.Contracts.Events;

namespace Todo.Client.Data.Services;

public interface ITrashService
{
	event AsyncEventHandler<TrashItemCreated>? ItemCreated;
	event AsyncEventHandler<TrashItemUpdated>? ItemUpdated;
	event AsyncEventHandler<TrashItemDeleted>? ItemDeleted;

	Task StartEventListener();
	Task<TrashItemModel[]> GetItemsAsync();
	Task RestoreItemAsync(ItemType type, Guid id);
	Task DeleteItemAsync(ItemType type, Guid id);
	Task DeleteAllItemsAsync();
}
