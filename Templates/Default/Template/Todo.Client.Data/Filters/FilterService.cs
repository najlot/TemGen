using System;
using System.Threading.Tasks;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.Client.Data.Filters;

public sealed class FilterService(IFilterRepository repository) : IFilterService
{
	public Task<Filter[]> GetItemsAsync(ItemType targetType)
	{
		return repository.GetItemsAsync(targetType);
	}

	public Task AddItemAsync(Filter item)
	{
		return repository.AddItemAsync(item);
	}

	public Task UpdateItemAsync(Filter item)
	{
		return repository.UpdateItemAsync(item);
	}

	public Task DeleteItemAsync(Guid id)
	{
		return repository.DeleteItemAsync(id);
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>