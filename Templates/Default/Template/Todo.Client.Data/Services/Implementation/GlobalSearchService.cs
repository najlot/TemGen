using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.Client.Data.Repositories;

namespace <# Project.Namespace#>.Client.Data.Services.Implementation;

public sealed class GlobalSearchService(IGlobalSearchRepository repository) : IGlobalSearchService
{
	public async Task<GlobalSearchItemModel[]> SearchAsync(string text)
	{
		return await repository.SearchAsync(text).ConfigureAwait(false);
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>