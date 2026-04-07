using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Service.Features.GlobalSearch;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GlobalSearchController(GlobalSearchService globalSearchService) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<GlobalSearchItem>>> Search([FromQuery] string text)
	{
		var items = await globalSearchService.SearchAsync(text).ConfigureAwait(false);
		return Ok(items);
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>