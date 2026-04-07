using System;
using System.Threading.Tasks;
using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Client.Data.Repositories;

public interface IHistoryRepository
{
	Task<HistoryEntry[]> GetItemsAsync(Guid entityId);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>