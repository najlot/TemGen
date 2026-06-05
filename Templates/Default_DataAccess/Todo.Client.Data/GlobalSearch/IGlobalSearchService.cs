using System.Threading.Tasks;

namespace <# Project.Namespace#>.Client.Data.GlobalSearch;

public interface IGlobalSearchService
{
	Task<GlobalSearchItemModel[]> SearchAsync(string text);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>