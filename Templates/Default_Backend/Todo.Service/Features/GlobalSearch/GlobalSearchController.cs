using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using <# Project.Namespace#>.Contracts.GlobalSearch;

namespace <# Project.Namespace#>.Service.Features.GlobalSearch;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GlobalSearchController(GlobalSearchService globalSearchService) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<GlobalSearchItem>>> Search([FromQuery] string text, CancellationToken cancellationToken)
	{
		var items = await globalSearchService.SearchAsync(text, cancellationToken).ConfigureAwait(false);
		return Ok(items);
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>