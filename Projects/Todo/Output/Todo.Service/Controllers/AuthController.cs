using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Todo.Contracts;
using Todo.Contracts.Commands;
using Todo.Service.Services;
using Todo.Service.Repository;

namespace Todo.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
	IUserService userService,
	TokenService tokenService) : ControllerBase
{
	[Authorize]
	[HttpGet("Refresh")]
	public ActionResult<string> Refresh()
	{
		if (!ModelState.IsValid)
		{
			return BadRequest();
		}

		var userId = User.GetUserId();
		var userName = User.Identity?.Name;

		if (userName is null)
		{
			return BadRequest();
		}

		var token = tokenService.GetRefreshToken(userName, userId);

		return Ok(token);
	}

	[AllowAnonymous]
	[HttpPost]
	public async Task<ActionResult<string>> Auth([FromBody] AuthRequest request)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest();
		}

		var token = await tokenService.GetToken(request.Username, request.Password).ConfigureAwait(false);

		if (token == null)
		{
			return Forbid();
		}

		return Ok(token);
	}

	[AllowAnonymous]
	[HttpPost("Register")]
	public async Task<ActionResult> Register(
		[FromBody] CreateUser command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest();
		}

		await userService.CreateUser(command, Guid.Empty).ConfigureAwait(false);
		await unitOfWork.CommitAsync().ConfigureAwait(false);

		return Ok();
	}
}