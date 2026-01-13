using System;
using System.Security.Claims;

namespace Todo.Service.Controllers;

public static class ClaimsPrincipalExtensions
{
	public static Guid GetUserId(this ClaimsPrincipal principal)
	{
		var name = principal.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrEmpty(name))
		{
			throw new InvalidOperationException("User id not found");
		}

		var userId = Guid.Parse(name);
		return userId;
	}
}