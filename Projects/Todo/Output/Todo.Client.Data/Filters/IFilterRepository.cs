using System;
using System.Threading.Tasks;
using Todo.Contracts.Filters;
using Todo.Contracts.Shared;

namespace Todo.Client.Data.Filters;

public interface IFilterRepository
{
	Task<Filter[]> GetItemsAsync(ItemType targetType);
	Task AddItemAsync(Filter item);
	Task UpdateItemAsync(Filter item);
	Task DeleteItemAsync(Guid id);
}
