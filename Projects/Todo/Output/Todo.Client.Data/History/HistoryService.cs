using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Contracts.History;

namespace Todo.Client.Data.History;

public sealed class HistoryService(
	IHistoryRepository repository,
	IEnumerable<IEntityHistoryLocalizer> historyLocalizers) : IHistoryService
{
	private readonly IEntityHistoryLocalizer[] _historyLocalizers = [.. historyLocalizers];

	public async Task<HistoryEntry[]> GetItemsAsync(Guid entityId, string entityName)
	{
		var items = await repository.GetItemsAsync(entityId).ConfigureAwait(false);
		var localizer = _historyLocalizers.FirstOrDefault(item => item.CanLocalize(entityName));
		if (localizer is not null)
		{
			await LocalizeHistoryPaths(items, localizer).ConfigureAwait(false);
		}

		return items
			.OrderByDescending(item => item.TimeStamp)
			.ToArray();
	}

	private static async Task LocalizeHistoryPaths(HistoryEntry[] items, IEntityHistoryLocalizer localizer)
	{
		foreach (var change in items.SelectMany(item => item.Changes))
		{
			var localizedChange = await localizer.Localize(change).ConfigureAwait(false);
			change.Path = localizedChange.Path;
			change.OldValue = localizedChange.OldValue;
			change.NewValue = localizedChange.NewValue;
		}
	}
}