using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Client.Data.Repositories;

namespace Todo.Client.Data.Services.Implementation;

public sealed class GlobalSearchService(IGlobalSearchRepository repository) : IGlobalSearchService
{
	public async Task<GlobalSearchItemModel[]> SearchAsync(string text)
	{
		return await repository.SearchAsync(text).ConfigureAwait(false);
	}
}
