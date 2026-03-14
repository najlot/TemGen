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
public class NoteController(NoteService noteService) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<NoteListItem>>> List()
	{
		var userId = User.GetUserId();
		var query = noteService.GetItemsForUserAsync(userId);
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<List<NoteListItem>>> ListFiltered(NoteFilter filter)
	{
		var userId = User.GetUserId();
		var query = noteService.GetItemsForUserAsync(filter, userId);
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<Note>> GetItem(Guid id)
	{
		var userId = User.GetUserId();
		var item = await noteService.GetItemAsync(id, userId).ConfigureAwait(false);
		if (item == null)
		{
			return NotFound();
		}

		return Ok(item);
	}

	[HttpPost]
	public async Task<ActionResult> Create(
		[FromBody] CreateNote command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var userId = User.GetUserId();
		await noteService.CreateNote(command, userId).ConfigureAwait(false);
		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}

	[HttpPut]
	public async Task<ActionResult> Update(
		[FromBody] UpdateNote command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var userId = User.GetUserId();
		await noteService.UpdateNote(command, userId).ConfigureAwait(false);
		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(Guid id, [FromServices] IUnitOfWork unitOfWork)
	{
		var userId = User.GetUserId();
		await noteService.DeleteNote(id, userId).ConfigureAwait(false);
		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}
}