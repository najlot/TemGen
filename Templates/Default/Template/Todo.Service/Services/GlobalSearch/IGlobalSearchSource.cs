using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Service.Services.GlobalSearch;

public interface IGlobalSearchSource
{
	IAsyncEnumerable<GlobalSearchItem> SearchAsync(string text);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>