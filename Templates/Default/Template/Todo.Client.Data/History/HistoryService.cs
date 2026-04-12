using System;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Contracts.History;

namespace <# Project.Namespace#>.Client.Data.History;

public sealed class HistoryService(IHistoryRepository repository) : IHistoryService
{
	public async Task<HistoryEntry[]> GetItemsAsync(Guid entityId)
	{
		var items = await repository.GetItemsAsync(entityId).ConfigureAwait(false);
		return items
			.OrderByDescending(item => item.TimeStamp)
			.ToArray();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>