using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Contracts;
using <#cs Write(Project.Namespace)#>.Service.Services;
using <#cs Write(Project.Namespace)#>.Contracts.Commands;
using <#cs Write(Project.Namespace)#>.Contracts.ListItems;
using <#cs Write(Project.Namespace)#>.Contracts.Filters;

namespace <#cs Write(Project.Namespace)#>.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class <#cs Write(Definition.Name)#>Controller : ControllerBase
{
	private readonly <#cs Write(Definition.Name)#>Service _<#cs Write(Definition.NameLow)#>Service;

	public <#cs Write(Definition.Name)#>Controller(<#cs Write(Definition.Name)#>Service <#cs Write(Definition.NameLow)#>Service)
	{
		_<#cs Write(Definition.NameLow)#>Service = <#cs Write(Definition.NameLow)#>Service;
	}

	[HttpGet]
	public async Task<ActionResult<List<<#cs Write(Definition.Name)#>ListItem>>> List()
	{
		var userId = User.GetUserId();
		var query = _<#cs Write(Definition.NameLow)#>Service.GetItemsForUserAsync(userId);
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<List<<#cs Write(Definition.Name)#>ListItem>>> ListFiltered(<#cs Write(Definition.Name)#>Filter filter)
	{
		var userId = User.GetUserId();
		var query = _<#cs Write(Definition.NameLow)#>Service.GetItemsForUserAsync(filter, userId);
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<<#cs Write(Definition.Name)#>>> GetItem(Guid id)
	{
		var userId = User.GetUserId();
		var item = await _<#cs Write(Definition.NameLow)#>Service.GetItemAsync(id, userId).ConfigureAwait(false);
		if (item == null)
		{
			return NotFound();
		}

		return Ok(item);
	}

	[HttpPost]
	public async Task<ActionResult> Create([FromBody] Create<#cs Write(Definition.Name)#> command)
	{
		var userId = User.GetUserId();
		await _<#cs Write(Definition.NameLow)#>Service.Create<#cs Write(Definition.Name)#>(command, userId).ConfigureAwait(false);
		return Ok();
	}

	[HttpPut]
	public async Task<ActionResult> Update([FromBody] Update<#cs Write(Definition.Name)#> command)
	{
		var userId = User.GetUserId();
		await _<#cs Write(Definition.NameLow)#>Service.Update<#cs Write(Definition.Name)#>(command, userId).ConfigureAwait(false);
		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(Guid id)
	{
		var userId = User.GetUserId();
		await _<#cs Write(Definition.NameLow)#>Service.Delete<#cs Write(Definition.Name)#>(id, userId).ConfigureAwait(false);
		return Ok();
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>