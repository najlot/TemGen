using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Models;

namespace <# Project.Namespace#>.Client.Data.Services;

public interface IGlobalSearchService
{
	Task<GlobalSearchItemModel[]> SearchAsync(string text);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>