using Todo.Service.Infrastructure.Persistence;

namespace Todo.Service.Features.History;

public interface IHistoryRepository : IEntityRepository<HistoryModel>
{
	Task<HistoryModel[]> GetHistoryEntries(Guid entityId);
	Task DeleteHistoryEntries(Guid entityId);
}
