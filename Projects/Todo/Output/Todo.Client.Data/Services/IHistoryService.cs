using System;
using System.Threading.Tasks;
using Todo.Contracts;

namespace Todo.Client.Data.Services;

public interface IHistoryService
{
	Task<HistoryEntry[]> GetItemsAsync(Guid entityId);
}