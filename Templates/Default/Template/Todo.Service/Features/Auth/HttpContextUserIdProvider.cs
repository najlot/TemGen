using System.Security.Claims;

namespace <# Project.Namespace#>.Service.Features.Auth;

public class HttpContextUserIdProvider(IHttpContextAccessor httpContextAccessor) : IUserIdProvider
{
	public Guid GetRequiredUserId()
	{
		var name = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrEmpty(name))
		{
			throw new InvalidOperationException("User id not found");
		}

		return Guid.Parse(name);
	}

	public string GetRequiredUsername()
	{
		var username = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

		if (string.IsNullOrWhiteSpace(username))
		{
			throw new InvalidOperationException("Username not found");
		}

		return username;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>