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
using Todo.Contracts.Filters;

namespace Todo.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TodoItemController : ControllerBase
{
	private readonly TodoItemService _todoItemService;

	public TodoItemController(TodoItemService todoItemService)
	{
		_todoItemService = todoItemService;
	}

	[HttpGet]
	public async Task<ActionResult<List<TodoItemListItem>>> List()
	{
		var userId = User.GetUserId();
		var query = _todoItemService.GetItemsForUserAsync(userId);
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<List<TodoItemListItem>>> ListFiltered(TodoItemFilter filter)
	{
		var userId = User.GetUserId();
		var query = _todoItemService.GetItemsForUserAsync(filter, userId);
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<TodoItem>> GetItem(Guid id)
	{
		var userId = User.GetUserId();
		var item = await _todoItemService.GetItemAsync(id, userId).ConfigureAwait(false);
		if (item == null)
		{
			return NotFound();
		}

		return Ok(item);
	}

	[HttpPost]
	public async Task<ActionResult> Create([FromBody] CreateTodoItem command)
	{
		var userId = User.GetUserId();
		await _todoItemService.CreateTodoItem(command, userId).ConfigureAwait(false);
		return Ok();
	}

	[HttpPut]
	public async Task<ActionResult> Update([FromBody] UpdateTodoItem command)
	{
		var userId = User.GetUserId();
		await _todoItemService.UpdateTodoItem(command, userId).ConfigureAwait(false);
		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(Guid id)
	{
		var userId = User.GetUserId();
		await _todoItemService.DeleteTodoItem(id, userId).ConfigureAwait(false);
		return Ok();
	}
}