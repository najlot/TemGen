using System.Threading.Tasks;

namespace <# Project.Namespace#>.Client.Data.GlobalSearch;

public sealed class GlobalSearchService(IGlobalSearchRepository repository) : IGlobalSearchService
{
	public async Task<GlobalSearchItemModel[]> SearchAsync(string text)
	{
		return await repository.SearchAsync(text).ConfigureAwait(false);
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>