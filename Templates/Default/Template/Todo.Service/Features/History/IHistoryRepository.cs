using <# Project.Namespace#>.Service.Infrastructure.Persistence;

namespace <# Project.Namespace#>.Service.Features.History;

public interface IHistoryRepository : IEntityRepository<HistoryModel>
{
	Task<HistoryModel[]> GetHistoryEntries(Guid entityId);
	Task DeleteHistoryEntries(Guid entityId);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>