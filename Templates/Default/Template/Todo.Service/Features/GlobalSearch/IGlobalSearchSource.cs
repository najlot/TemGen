using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Service.Features.GlobalSearch;

public interface IGlobalSearchSource
{
	IAsyncEnumerable<GlobalSearchItem> SearchAsync(string text);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>