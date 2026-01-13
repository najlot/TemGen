using Cosei.Client.Base;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Todo.Contracts;

namespace Todo.Client.Data.Identity;

public class TokenService : ITokenService
{
	private readonly IRequestClient _client;

	public TokenService(IRequestClient client)
	{
		_client = client;
	}

	public async Task<string> CreateToken(string username, string password)
	{
		var request = new AuthRequest
		{
			Username = username,
			Password = password
		};

		var response = await _client.PostAsync("api/Auth", JsonSerializer.Serialize(request), "application/json");

		if (response.StatusCode >= 200 && response.StatusCode < 300)
		{
			var token = Encoding.UTF8.GetString(response.Body.ToArray());
			return token;
		}

		return string.Empty;
	}
}