using Microsoft.AspNetCore.Components.Authorization;

namespace <# Project.Namespace#>.Blazor.Identity;

public interface IAuthenticationService
{
	Task<AuthenticationState> GetAuthenticationStateAsync();

	Task LoginAsync(string username, string token);

	Task LogoutAsync();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>