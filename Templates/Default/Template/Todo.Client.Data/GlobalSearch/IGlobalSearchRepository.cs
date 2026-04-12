using System.Threading.Tasks;

namespace <# Project.Namespace#>.Client.Data.GlobalSearch;

public interface IGlobalSearchRepository
{
	Task<GlobalSearchItemModel[]> SearchAsync(string text);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>