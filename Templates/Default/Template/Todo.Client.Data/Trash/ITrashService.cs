using System;
using System.Threading.Tasks;
using <# Project.Namespace#>.Contracts.Trash;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.Client.Data.Trash;

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
<#cs SetOutputPathAndSkipOtherDefinitions()#>