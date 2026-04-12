using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Contracts.History;
using Todo.Service.Shared.Results;

namespace Todo.Service.Features.History;

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
