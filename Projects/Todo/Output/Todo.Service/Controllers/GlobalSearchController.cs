using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Contracts;
using Todo.Service.Services.GlobalSearch;

namespace Todo.Service.Controllers;

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
