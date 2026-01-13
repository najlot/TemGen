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

namespace Todo.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
	private readonly IUserService _userService;

	public UserController(IUserService userService)
	{
		_userService = userService;
	}

	[HttpGet]
	public async Task<ActionResult<List<UserListItem>>> List()
	{
		var userId = User.GetUserId();
		var query = _userService.GetItemsForUser(userId);
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpGet("Current")]
	public async Task<ActionResult<User>> GetCurrentUser()
	{
		var userId = User.GetUserId();
		var item = await _userService.GetItem(userId).ConfigureAwait(false);
		if (item == null)
		{
			return NotFound();
		}

		return Ok(item);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<User>> GetItem(Guid id)
	{
		var item = await _userService.GetItem(id).ConfigureAwait(false);
		if (item == null)
		{
			return NotFound();
		}

		return Ok(item);
	}

	[HttpPost]
	public async Task<ActionResult> Create([FromBody] CreateUser command)
	{
		var userId = User.GetUserId();
		await _userService.CreateUser(command, userId).ConfigureAwait(false);
		return Ok();
	}

	[HttpPut]
	public async Task<ActionResult> Update([FromBody] UpdateUser command)
	{
		var userId = User.GetUserId();
		await _userService.UpdateUser(command, userId).ConfigureAwait(false);
		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(Guid id)
	{
		var userId = User.GetUserId();
		await _userService.DeleteUser(id, userId).ConfigureAwait(false);
		return Ok();
	}
}