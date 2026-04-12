using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Contracts.TodoItems;
using Todo.Service.Infrastructure.Persistence;
using Todo.Service.Shared.Results;

namespace Todo.Service.Features.TodoItems;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TodoItemController(TodoItemService todoItemService) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<TodoItemListItem>>> List()
	{
		var query = todoItemService.GetItemsForUserAsync();
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<List<TodoItemListItem>>> ListFiltered(TodoItemFilter filter)
	{
		var query = todoItemService.GetItemsForUserAsync(filter);
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<TodoItem>> GetItem(Guid id)
	{
		var result = await todoItemService.GetItemAsync(id).ConfigureAwait(false);
		return this.ToActionResult(result);
	}

	[HttpPost]
	public async Task<ActionResult> Create(
		[FromBody] CreateTodoItem command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var result = await todoItemService.CreateTodoItem(command).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}

	[HttpPut]
	public async Task<ActionResult> Update(
		[FromBody] UpdateTodoItem command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var result = await todoItemService.UpdateTodoItem(command).ConfigureAwait(false);
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
		var result = await todoItemService.DeleteTodoItem(id).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}
}