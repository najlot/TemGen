using System;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Client.Data.Repositories;

public interface ITrashRepository
{
	Task<TrashItemModel[]> GetItemsAsync();
	Task RestoreItemAsync(ItemType type, Guid id);
	Task DeleteItemAsync(ItemType type, Guid id);
	Task DeleteAllItemsAsync();
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>