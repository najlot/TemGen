using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Contracts;
using Todo.Service.Services;
using Todo.Contracts.Commands;
using Todo.Contracts.ListItems;
using Todo.Service.Repository;

namespace Todo.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<UserListItem>>> List()
	{
		var userId = User.GetUserId();
		var query = userService.GetItemsForUser(userId);
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpGet("Current")]
	public async Task<ActionResult<User>> GetCurrentUser()
	{
		var userId = User.GetUserId();
		var item = await userService.GetItem(userId).ConfigureAwait(false);
		if (item == null)
		{
			return NotFound();
		}

		return Ok(item);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<User>> GetItem(Guid id)
	{
		var item = await userService.GetItem(id).ConfigureAwait(false);
		if (item == null)
		{
			return NotFound();
		}

		return Ok(item);
	}

	[HttpPost]
	public async Task<ActionResult> Create(
		[FromBody] CreateUser command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var userId = User.GetUserId();
		await userService.CreateUser(command, userId).ConfigureAwait(false);
		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}

	[HttpPut]
	public async Task<ActionResult> Update(
		[FromBody] UpdateUser command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var userId = User.GetUserId();
		await userService.UpdateUser(command, userId).ConfigureAwait(false);
		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(Guid id, [FromServices] IUnitOfWork unitOfWork)
	{
		var userId = User.GetUserId();
		await userService.DeleteUser(id, userId).ConfigureAwait(false);
		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}
}