using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Todo.Contracts.Filters;
using Todo.Contracts.Shared;
using Todo.Service.Infrastructure.Persistence;
using Todo.Service.Shared.Results;

namespace Todo.Service.Features.Filters;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public sealed class FiltersController(FilterService filterService) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<Filter[]>> List([FromQuery] ItemType targetType)
	{
		var items = await filterService.GetItemsAsync(targetType).ToArrayAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpPost]
	public async Task<ActionResult> Create(
		[FromBody] CreateFilter command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var result = await filterService.CreateFilterAsync(command).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}

	[HttpPut]
	public async Task<ActionResult> Update(
		[FromBody] UpdateFilter command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var result = await filterService.UpdateFilterAsync(command).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(Guid id, [FromServices] IUnitOfWork unitOfWork)
	{
		var result = await filterService.DeleteItemAsync(id).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}
}
