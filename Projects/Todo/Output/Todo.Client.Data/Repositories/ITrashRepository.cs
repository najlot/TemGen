using System;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Contracts;

namespace Todo.Client.Data.Repositories;

public interface ITrashRepository
{
	Task<TrashItemModel[]> GetItemsAsync();
	Task RestoreItemAsync(ItemType type, Guid id);
	Task DeleteItemAsync(ItemType type, Guid id);
	Task DeleteAllItemsAsync();
}
