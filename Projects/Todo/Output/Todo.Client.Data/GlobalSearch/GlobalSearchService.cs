using System.Threading;
using System.Threading.Tasks;

namespace Todo.Client.Data.GlobalSearch;

public sealed class GlobalSearchService(IGlobalSearchRepository repository) : IGlobalSearchService
{
	public async Task<GlobalSearchItemModel[]> SearchAsync(string text, CancellationToken cancellationToken = default)
	{
		return await repository.SearchAsync(text, cancellationToken).ConfigureAwait(false);
	}
}
