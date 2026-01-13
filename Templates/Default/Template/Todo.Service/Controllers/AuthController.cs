using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Contracts;
using <#cs Write(Project.Namespace)#>.Contracts.Commands;
using <#cs Write(Project.Namespace)#>.Service.Services;

namespace <#cs Write(Project.Namespace)#>.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly IUserService _userService;
	private readonly TokenService _tokenService;

	public AuthController(
		IUserService userService,
		TokenService tokenService)
	{
		_userService = userService;
		_tokenService = tokenService;
	}

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

		var token = _tokenService.GetRefreshToken(userName, userId);

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

		var token = await _tokenService.GetToken(request.Username, request.Password).ConfigureAwait(false);

		if (token == null)
		{
			return Forbid();
		}

		return Ok(token);
	}

	[AllowAnonymous]
	[HttpPost("Register")]
	public async Task<ActionResult> Register([FromBody] CreateUser command)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest();
		}

		await _userService.CreateUser(command, Guid.Empty).ConfigureAwait(false);

		return Ok();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>