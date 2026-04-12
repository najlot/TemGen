using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using <# Project.Namespace#>.Contracts.Users;
using <# Project.Namespace#>.Service.Infrastructure.Persistence;
using <# Project.Namespace#>.Service.Shared.Results;

namespace <# Project.Namespace#>.Service.Features.Users;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<UserListItem>>> List()
	{
		var query = userService.GetItemsForUser();
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpGet("Current")]
	public async Task<ActionResult<User>> GetCurrentUser()
	{
		var result = await userService.GetCurrentUser().ConfigureAwait(false);
		return this.ToActionResult(result);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<User>> GetItem(Guid id)
	{
		var result = await userService.GetItem(id).ConfigureAwait(false);
		return this.ToActionResult(result);
	}

	[HttpPost]
	public async Task<ActionResult> Create(
		[FromBody] CreateUser command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var result = await userService.CreateUser(command).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}

	[HttpPut]
	public async Task<ActionResult> Update(
		[FromBody] UpdateUser command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var result = await userService.UpdateUser(command).ConfigureAwait(false);
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
		var result = await userService.DeleteUser(id).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>