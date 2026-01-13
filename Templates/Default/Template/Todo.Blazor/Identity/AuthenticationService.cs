using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using <#cs Write(Project.Namespace)#>.Blazor.Services;
using <#cs Write(Project.Namespace)#>.Client.Data.Identity;

namespace <#cs Write(Project.Namespace)#>.Blazor.Identity;

public class AuthenticationService : AuthenticationStateProvider, IAuthenticationService
{
	private readonly IUserDataStore _userDataStore;
	private readonly ISubscriberProvider _subscriberProvider;
	private readonly ILogger _logger;

	public AuthenticationService(IUserDataStore userDataStore, ISubscriberProvider subscriberProvider, ILogger<AuthenticationService> logger)
	{
		_userDataStore = userDataStore;
		_subscriberProvider = subscriberProvider;
		_logger = logger;
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		try
		{
			var username = await _userDataStore.GetUsername();
			var token = await _userDataStore.GetAccessToken();

			if (!string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(username))
			{
				var securityToken = new JwtSecurityToken(token);
				var validTo = securityToken.Payload.ValidTo;

				if (validTo > DateTime.UtcNow)
				{
					return await GenerateAuthenticationState(username);
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Could not retrieve authentication state.");
			await LogoutAsync();
		}

		return await GenerateEmptyAuthenticationState();
	}

	public async Task LoginAsync(string username, string token)
	{
		await _userDataStore.SetUserData(username, token);
		_ = await _subscriberProvider.GetSubscriber();

		NotifyAuthenticationStateChanged(GenerateAuthenticationState(username));
	}

	public async Task LogoutAsync()
	{
		await _userDataStore.SetUserData("", "");
		await _subscriberProvider.ClearSubscriber();

		NotifyAuthenticationStateChanged(GenerateEmptyAuthenticationState());
	}

	private Task<AuthenticationState> GenerateAuthenticationState(string username)
	{
		var claimsIdentity = new ClaimsIdentity(new[]
		{
			new Claim(ClaimTypes.Name, username),
		}, "auth");

		var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
		return Task.FromResult(new AuthenticationState(claimsPrincipal));
	}

	private Task<AuthenticationState> GenerateEmptyAuthenticationState()
	{
		return Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>