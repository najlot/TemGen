using System;
using System.Threading.Tasks;
using <# Project.Namespace#>.Contracts.History;

namespace <# Project.Namespace#>.Client.Data.History;

public interface IHistoryRepository
{
	Task<HistoryEntry[]> GetItemsAsync(Guid entityId);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>