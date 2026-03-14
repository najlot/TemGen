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
using Todo.Service.Repository;

namespace Todo.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TodoItemController(TodoItemService todoItemService) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<TodoItemListItem>>> List()
	{
		var userId = User.GetUserId();
		var query = todoItemService.GetItemsForUserAsync(userId);
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<List<TodoItemListItem>>> ListFiltered(TodoItemFilter filter)
	{
		var userId = User.GetUserId();
		var query = todoItemService.GetItemsForUserAsync(filter, userId);
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<TodoItem>> GetItem(Guid id)
	{
		var userId = User.GetUserId();
		var item = await todoItemService.GetItemAsync(id, userId).ConfigureAwait(false);
		if (item == null)
		{
			return NotFound();
		}

		return Ok(item);
	}

	[HttpPost]
	public async Task<ActionResult> Create(
		[FromBody] CreateTodoItem command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var userId = User.GetUserId();
		await todoItemService.CreateTodoItem(command, userId).ConfigureAwait(false);
		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}

	[HttpPut]
	public async Task<ActionResult> Update(
		[FromBody] UpdateTodoItem command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var userId = User.GetUserId();
		await todoItemService.UpdateTodoItem(command, userId).ConfigureAwait(false);
		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(Guid id, [FromServices] IUnitOfWork unitOfWork)
	{
		var userId = User.GetUserId();
		await todoItemService.DeleteTodoItem(id, userId).ConfigureAwait(false);
		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}
}