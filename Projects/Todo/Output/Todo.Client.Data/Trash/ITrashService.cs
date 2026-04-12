using System;
using System.Threading.Tasks;
using Todo.Contracts.Trash;
using Todo.Contracts.Shared;

namespace Todo.Client.Data.Trash;

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
