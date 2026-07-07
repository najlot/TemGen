using System.Collections.Generic;
using System.Threading;
using <# Project.Namespace#>.Contracts.GlobalSearch;

namespace <# Project.Namespace#>.Service.Features.GlobalSearch;

public interface IGlobalSearchSource
{
	IAsyncEnumerable<GlobalSearchItem> SearchAsync(string text, CancellationToken cancellationToken = default);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>