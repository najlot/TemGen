using System.Security.Claims;

namespace Todo.Service.Services;

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
}
