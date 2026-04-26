using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Todo.Client.Data;
using Todo.Contracts.Auth;

namespace Todo.Client.Data.Identity;

public class TokenService(IHttpClientFactory httpClientFactory) : ITokenService
{
	public async Task<string> CreateToken(string username, string password)
	{
		var request = new AuthRequest
		{
			Username = username,
			Password = password
		};

		using var client = httpClientFactory.CreateClient();
		var response = await client.PostAsJsonAsync("api/Auth", request, ClientDataJsonSerializer.Options);

		if (response.IsSuccessStatusCode)
		{
			var token = await response.Content.ReadAsStringAsync();
			return token;
		}

		return string.Empty;
	}
}