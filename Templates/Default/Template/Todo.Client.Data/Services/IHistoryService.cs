using System;
using System.Threading.Tasks;
using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Client.Data.Services;

public interface IHistoryService
{
	Task<HistoryEntry[]> GetItemsAsync(Guid entityId);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>