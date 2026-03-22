using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Contracts.Commands;
using <# Project.Namespace#>.Service.Services;
using <# Project.Namespace#>.Service.Repository;

namespace <# Project.Namespace#>.Service.Controllers;

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

		var result = await userService.CreateUser(command).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);

		return Ok();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>