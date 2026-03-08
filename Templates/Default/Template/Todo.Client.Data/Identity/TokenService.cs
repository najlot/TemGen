using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Contracts;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Identity;

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
		var response = await client.PostAsJsonAsync("api/Auth", request);

		if (response.IsSuccessStatusCode)
		{
			var token = await response.Content.ReadAsStringAsync();
			return token;
		}

		return string.Empty;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>