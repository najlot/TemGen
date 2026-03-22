using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Contracts;
using Todo.Service.Repository;
using Todo.Service.Services.Trash;

namespace Todo.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TrashController(TrashService trashService) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<TrashItem>>> List()
	{
		var items = await trashService.GetItemsAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpPost("Restore/{type}/{id}")]
	public async Task<ActionResult> Restore(ItemType type, Guid id, [FromServices] IUnitOfWork unitOfWork)
	{
		var result = await trashService.RestoreAsync(type, id).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}

	[HttpDelete("{type}/{id}")]
	public async Task<ActionResult> Delete(ItemType type, Guid id, [FromServices] IUnitOfWork unitOfWork)
	{
		var result = await trashService.DeleteAsync(type, id).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}

	[HttpDelete("All")]
	public async Task<ActionResult> DeleteAll([FromServices] IUnitOfWork unitOfWork)
	{
		await trashService.DeleteAllAsync().ConfigureAwait(false);
		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}
}
