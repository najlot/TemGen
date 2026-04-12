using <# Project.Namespace#>.Contracts.Trash;
using <# Project.Namespace#>.Contracts.Shared;
using <# Project.Namespace#>.Service.Shared.Results;

namespace <# Project.Namespace#>.Service.Features.Trash;

public interface ITrashSource
{
	ItemType Type { get; }

	IAsyncEnumerable<TrashItem> GetItemsAsync();

	Task<Result> RestoreAsync(Guid id);

	Task<Result> DeleteAsync(Guid id);

	Task DeleteAllAsync();
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>