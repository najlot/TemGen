using System;
using System.Threading.Tasks;
using Todo.Contracts;

namespace Todo.Client.Data.Repositories;

public interface IHistoryRepository
{
	Task<HistoryEntry[]> GetItemsAsync(Guid entityId);
}