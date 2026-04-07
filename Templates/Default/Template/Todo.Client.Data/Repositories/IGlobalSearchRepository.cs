using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Models;

namespace <# Project.Namespace#>.Client.Data.Repositories;

public interface IGlobalSearchRepository
{
	Task<GlobalSearchItemModel[]> SearchAsync(string text);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>