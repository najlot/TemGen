using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using <# Project.Namespace#>.Client.Data.Identity;

namespace <# Project.Namespace#>.Htmx.Identity;

public class HttpContextUserDataStore : IUserDataStore
{
	private const string TokenClaimType = "access_token";

	private readonly IHttpContextAccessor _httpContextAccessor;

	public HttpContextUserDataStore(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public Task<string?> GetAccessToken()
	{
		var token = _httpContextAccessor.HttpContext?.User.FindFirst(TokenClaimType)?.Value;
		return Task.FromResult(token);
	}

	public Task<string?> GetUsername()
	{
		var username = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;
		return Task.FromResult(username);
	}

	public async Task SetAccessToken(string token)
	{
		var context = _httpContextAccessor.HttpContext;

		if (context is null)
		{
			return;
		}

		var username = context.User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
		await SignInAsync(context, username, token);
	}

	public async Task SetUsername(string username)
	{
		var context = _httpContextAccessor.HttpContext;

		if (context is null)
		{
			return;
		}

		var token = context.User.FindFirst(TokenClaimType)?.Value ?? string.Empty;
		await SignInAsync(context, username, token);
	}

	public async Task SetUserData(string username, string token)
	{
		var context = _httpContextAccessor.HttpContext;

		if (context is null)
		{
			return;
		}

		if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(token))
		{
			await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return;
		}

		await SignInAsync(context, username, token);
	}

	public static async Task SignInAsync(HttpContext context, string username, string token)
	{
		var claims = new List<Claim>
		{
			new(ClaimTypes.Name, username),
			new(TokenClaimType, token),
		};

		var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		var principal = new ClaimsPrincipal(identity);

		await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
		{
			IsPersistent = true,
			ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
		});
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
