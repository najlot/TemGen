using System;
using System.Threading.Tasks;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.Client.Data.Trash;

public interface ITrashRepository
{
	Task<TrashItemModel[]> GetItemsAsync();
	Task RestoreItemAsync(ItemType type, Guid id);
	Task DeleteItemAsync(ItemType type, Guid id);
	Task DeleteAllItemsAsync();
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>