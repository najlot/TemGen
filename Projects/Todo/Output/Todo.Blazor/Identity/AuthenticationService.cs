using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Todo.Client.Data.Identity;

namespace Todo.Blazor.Identity;

public class AuthenticationService : AuthenticationStateProvider, IAuthenticationService
{
	private const string AuthenticationType = "Bearer";
	private const string TokenIssuer = "Todo.Service";
	private const string TokenAudience = TokenIssuer;

	private readonly IUserDataStore _userDataStore;
	private readonly ILogger _logger;

	public AuthenticationService(IUserDataStore userDataStore, ILogger<AuthenticationService> logger)
	{
		_userDataStore = userDataStore;
		_logger = logger;
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		try
		{
			var token = await _userDataStore.GetAccessToken();

			if (TryCreateAuthenticationState(token, out var authenticationState, out _))
			{
				return authenticationState;
			}

			if (!string.IsNullOrWhiteSpace(token))
			{
				await _userDataStore.SetUserData(string.Empty, string.Empty);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Could not retrieve authentication state.");
			await _userDataStore.SetUserData(string.Empty, string.Empty);
		}

		return GenerateEmptyAuthenticationState();
	}

	public async Task LoginAsync(string username, string token)
	{
		if (!TryCreateAuthenticationState(token, out var authenticationState, out var tokenUsername))
		{
			throw new InvalidOperationException("Authentication token is invalid.");
		}

		await _userDataStore.SetUserData(tokenUsername ?? username, token);

		NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
	}

	public async Task LogoutAsync()
	{
		await _userDataStore.SetUserData("", "");

		NotifyAuthenticationStateChanged(Task.FromResult(GenerateEmptyAuthenticationState()));
	}

	private static bool TryCreateAuthenticationState(string? token, out AuthenticationState authenticationState, out string? username)
	{
		authenticationState = GenerateEmptyAuthenticationState();
		username = null;

		if (string.IsNullOrWhiteSpace(token))
		{
			return false;
		}

		var securityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

		if (securityToken.ValidTo <= DateTime.UtcNow)
		{
			return false;
		}

		if (!string.Equals(securityToken.Issuer, TokenIssuer, StringComparison.Ordinal))
		{
			return false;
		}

		if (!securityToken.Audiences.Contains(TokenAudience, StringComparer.Ordinal))
		{
			return false;
		}

		var claims = securityToken.Claims.ToList();
		username = claims
			.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value
			?? claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.UniqueName)?.Value
			?? claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Name)?.Value
			?? claims.FirstOrDefault(claim => claim.Type == "name")?.Value;

		if (string.IsNullOrWhiteSpace(username))
		{
			return false;
		}

		if (!claims.Any(claim => claim.Type == ClaimTypes.Name))
		{
			claims.Add(new Claim(ClaimTypes.Name, username));
		}

		var claimsIdentity = new ClaimsIdentity(claims, AuthenticationType, ClaimTypes.Name, ClaimTypes.Role);
		authenticationState = new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
		return true;
	}

	private static AuthenticationState GenerateEmptyAuthenticationState()
	{
		return new AuthenticationState(new ClaimsPrincipal());
	}
}