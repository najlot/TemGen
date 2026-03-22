using System;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Contracts.Events;

namespace <# Project.Namespace#>.Client.Data.Services;

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