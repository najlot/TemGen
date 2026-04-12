using System;
using System.Threading.Tasks;
using Todo.Contracts.Shared;

namespace Todo.Client.Data.Trash;

public interface ITrashRepository
{
	Task<TrashItemModel[]> GetItemsAsync();
	Task RestoreItemAsync(ItemType type, Guid id);
	Task DeleteItemAsync(ItemType type, Guid id);
	Task DeleteAllItemsAsync();
}
