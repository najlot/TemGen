using Microsoft.AspNetCore.Components.Authorization;

namespace <#cs Write(Project.Namespace)#>.Blazor.Identity;

public interface IAuthenticationService
{
	Task<AuthenticationState> GetAuthenticationStateAsync();

	Task LoginAsync(string username, string token);

	Task LogoutAsync();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>