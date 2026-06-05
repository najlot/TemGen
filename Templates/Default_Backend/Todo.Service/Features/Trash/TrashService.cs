using <# Project.Namespace#>.Contracts.Trash;
using <# Project.Namespace#>.Contracts.Shared;
using <# Project.Namespace#>.Service.Shared.Results;

namespace <# Project.Namespace#>.Service.Features.Trash;

public class TrashService(IEnumerable<ITrashSource> sources)
{
	private readonly Dictionary<ItemType, ITrashSource> _sourcesByType = sources.ToDictionary(source => source.Type);

	public async Task<TrashItem[]> GetItemsAsync()
	{
		var items = new List<TrashItem>();

		foreach (var source in sources)
		{
			var sourceItems = await source.GetItemsAsync().ToListAsync().ConfigureAwait(false);
			items.AddRange(sourceItems);
		}

		return items.OrderByDescending(item => item.DeletedAt).ToArray();
	}

	public async Task<Result> RestoreAsync(ItemType type, Guid id)
	{
		if (!_sourcesByType.TryGetValue(type, out var source))
		{
			return Result.NotFound("Trash item source not found!");
		}

		return await source.RestoreAsync(id).ConfigureAwait(false);
	}

	public async Task<Result> DeleteAsync(ItemType type, Guid id)
	{
		if (!_sourcesByType.TryGetValue(type, out var source))
		{
			return Result.NotFound("Trash item source not found!");
		}

		return await source.DeleteAsync(id).ConfigureAwait(false);
	}

	public async Task DeleteAllAsync()
	{
		foreach (var source in _sourcesByType.Values)
		{
			await source.DeleteAllAsync().ConfigureAwait(false);
		}
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>