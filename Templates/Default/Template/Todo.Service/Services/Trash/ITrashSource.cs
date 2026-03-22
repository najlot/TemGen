using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Service.Services.Trash;

public interface ITrashSource
{
	ItemType Type { get; }

	IAsyncEnumerable<TrashItem> GetItemsAsync();

	Task<Result> RestoreAsync(Guid id);

	Task<Result> DeleteAsync(Guid id);

	Task DeleteAllAsync();
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>