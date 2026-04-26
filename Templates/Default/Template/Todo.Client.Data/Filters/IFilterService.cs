using System;
using System.Threading.Tasks;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.Client.Data.Filters;

public interface IFilterService
{
	Task<Filter[]> GetItemsAsync(ItemType targetType);
	Task AddItemAsync(Filter item);
	Task UpdateItemAsync(Filter item);
	Task DeleteItemAsync(Guid id);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>