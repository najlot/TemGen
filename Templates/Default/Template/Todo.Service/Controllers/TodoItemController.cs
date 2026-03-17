using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Service.Services;
using <# Project.Namespace#>.Contracts.Commands;
using <# Project.Namespace#>.Contracts.ListItems;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Service.Repository;

namespace <# Project.Namespace#>.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class <# Definition.Name#>Controller(<# Definition.Name#>Service <# Definition.NameLow#>Service) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<<# Definition.Name#>ListItem>>> List()
	{
		var userId = User.GetUserId();
		var query = <# Definition.NameLow#>Service.GetItemsForUserAsync(userId);
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<List<<# Definition.Name#>ListItem>>> ListFiltered(<# Definition.Name#>Filter filter)
	{
		var userId = User.GetUserId();
		var query = <# Definition.NameLow#>Service.GetItemsForUserAsync(filter, userId);
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<<# Definition.Name#>>> GetItem(Guid id)
	{
		var userId = User.GetUserId();
		var item = await <# Definition.NameLow#>Service.GetItemAsync(id, userId).ConfigureAwait(false);
		if (item == null)
		{
			return NotFound();
		}

		return Ok(item);
	}

	[HttpPost]
	public async Task<ActionResult> Create(
		[FromBody] Create<# Definition.Name#> command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var userId = User.GetUserId();
		await <# Definition.NameLow#>Service.Create<# Definition.Name#>(command, userId).ConfigureAwait(false);
		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}

	[HttpPut]
	public async Task<ActionResult> Update(
		[FromBody] Update<# Definition.Name#> command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var userId = User.GetUserId();
		await <# Definition.NameLow#>Service.Update<# Definition.Name#>(command, userId).ConfigureAwait(false);
		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(Guid id, [FromServices] IUnitOfWork unitOfWork)
	{
		var userId = User.GetUserId();
		await <# Definition.NameLow#>Service.Delete<# Definition.Name#>(id, userId).ConfigureAwait(false);
		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>