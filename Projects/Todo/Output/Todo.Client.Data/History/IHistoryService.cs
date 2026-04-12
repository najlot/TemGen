using System;
using System.Threading.Tasks;
using Todo.Contracts.History;

namespace Todo.Client.Data.History;

public interface IHistoryService
{
	Task<HistoryEntry[]> GetItemsAsync(Guid entityId);
}