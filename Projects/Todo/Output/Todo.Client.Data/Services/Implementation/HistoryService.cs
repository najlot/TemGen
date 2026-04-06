using System;
using System.Linq;
using System.Threading.Tasks;
using Todo.Client.Data.Repositories;
using Todo.Contracts;

namespace Todo.Client.Data.Services.Implementation;

public sealed class HistoryService(IHistoryRepository repository) : IHistoryService
{
	public async Task<HistoryEntry[]> GetItemsAsync(Guid entityId)
	{
		var items = await repository.GetItemsAsync(entityId).ConfigureAwait(false);
		return items
			.OrderByDescending(item => item.TimeStamp)
			.ToArray();
	}
}