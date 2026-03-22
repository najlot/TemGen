using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
public class NoteController(NoteService noteService) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<NoteListItem>>> List()
	{
		var query = noteService.GetItemsForUserAsync();
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<List<NoteListItem>>> ListFiltered(NoteFilter filter)
	{
		var query = noteService.GetItemsForUserAsync(filter);
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<Note>> GetItem(Guid id)
	{
		var result = await noteService.GetItemAsync(id).ConfigureAwait(false);
		return this.ToActionResult(result);
	}

	[HttpPost]
	public async Task<ActionResult> Create(
		[FromBody] CreateNote command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var result = await noteService.CreateNote(command).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}

	[HttpPut]
	public async Task<ActionResult> Update(
		[FromBody] UpdateNote command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var result = await noteService.UpdateNote(command).ConfigureAwait(false);
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
		var result = await noteService.DeleteNote(id).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}
}