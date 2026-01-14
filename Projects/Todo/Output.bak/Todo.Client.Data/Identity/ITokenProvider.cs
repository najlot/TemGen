using System.Collections.Generic;
using System.Threading.Tasks;

namespace Todo.Client.Data.Identity;

public interface ITokenProvider
{
	Task<string> GetToken();
}

public static class TokenProviderExtensions
{
	public static async Task<Dictionary<string, string>> GetAuthorizationHeaders(this ITokenProvider tokenProvider)
	{
		var token = await tokenProvider.GetToken().ConfigureAwait(false);
		return new Dictionary<string, string>
		{
			{ "Authorization", $"Bearer {token}" }
		};
	}
}