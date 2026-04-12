using Todo.Contracts.Trash;
using Todo.Contracts.Shared;
using Todo.Service.Shared.Results;

namespace Todo.Service.Features.Trash;

public interface ITrashSource
{
	ItemType Type { get; }

	IAsyncEnumerable<TrashItem> GetItemsAsync();

	Task<Result> RestoreAsync(Guid id);

	Task<Result> DeleteAsync(Guid id);

	Task DeleteAllAsync();
}
