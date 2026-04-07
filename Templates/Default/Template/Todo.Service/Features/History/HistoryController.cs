using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Service.Shared.Results;

namespace <# Project.Namespace#>.Service.Features.History;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class HistoryController(HistoryService historyService) : ControllerBase
{
	[HttpGet("{id}")]
	public async Task<ActionResult<HistoryEntry[]>> GetItem(Guid id)
	{
		var result = await historyService.GetHistoryEntries(id).ConfigureAwait(false);
		return this.ToActionResult(result);
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>