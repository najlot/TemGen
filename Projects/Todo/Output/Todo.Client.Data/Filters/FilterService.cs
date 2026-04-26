using System;
using System.Threading.Tasks;
using Todo.Contracts.Filters;
using Todo.Contracts.Shared;

namespace Todo.Client.Data.Filters;

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
