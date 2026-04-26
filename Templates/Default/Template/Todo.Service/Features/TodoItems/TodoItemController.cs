using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Contracts.<# Definition.Name#>s;
using <# Project.Namespace#>.Service.Infrastructure.Persistence;
using <# Project.Namespace#>.Service.Shared.Results;

namespace <# Project.Namespace#>.Service.Features.<# Definition.Name#>s;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class <# Definition.Name#>Controller(<# Definition.Name#>Service <# Definition.NameLow#>Service) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<<# Definition.Name#>ListItem>>> List()
	{
		var query = <# Definition.NameLow#>Service.GetItemsForUserAsync();
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<List<<# Definition.Name#>ListItem>>> ListFiltered(EntityFilter filter)
	{
		var query = <# Definition.NameLow#>Service.GetItemsForUserAsync(filter);
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<<# Definition.Name#>>> GetItem(Guid id)
	{
		var result = await <# Definition.NameLow#>Service.GetItemAsync(id).ConfigureAwait(false);
		return this.ToActionResult(result);
	}

	[HttpPost]
	public async Task<ActionResult> Create(
		[FromBody] Create<# Definition.Name#> command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var result = await <# Definition.NameLow#>Service.Create<# Definition.Name#>(command).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}

	[HttpPut]
	public async Task<ActionResult> Update(
		[FromBody] Update<# Definition.Name#> command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var result = await <# Definition.NameLow#>Service.Update<# Definition.Name#>(command).ConfigureAwait(false);
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
		var result = await <# Definition.NameLow#>Service.Delete<# Definition.Name#>(id).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>