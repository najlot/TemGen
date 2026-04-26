using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Contracts.Auth;
using Todo.Contracts.Users;
using Todo.Service.Features.Users;
using Todo.Service.Infrastructure.Persistence;
using Todo.Service.Shared.Results;

namespace Todo.Service.Features.Auth;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
	IUserService userService,
	PasswordResetService passwordResetService,
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

		var userName = User.Identity?.Name;

		if (userName is null)
		{
			return BadRequest();
		}

		var token = tokenService.GetRefreshToken(userName);

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
			return Unauthorized();
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

		var result = await userService.CreateUser(command).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);

		return Ok();
	}

	[AllowAnonymous]
	[HttpPost("RequestPasswordReset")]
	public async Task<ActionResult> RequestPasswordReset(
		[FromBody] RequestPasswordReset request,
		[FromServices] IUnitOfWork unitOfWork)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest();
		}

		var result = await passwordResetService.RequestPasswordReset(request).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);

		return Ok();
	}

	[AllowAnonymous]
	[HttpPost("ResetPassword")]
	public async Task<ActionResult> ResetPassword(
		[FromBody] ResetPassword request,
		[FromServices] IUnitOfWork unitOfWork)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest();
		}

		var result = await passwordResetService.ResetPassword(request).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);

		return Ok();
	}
}