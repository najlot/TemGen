using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading.Tasks;

namespace Todo.Client.Data.Identity;

public class RefreshingTokenProvider(IHttpClientFactory httpClientFactory, IUserDataStore userDataStore)
	: ITokenProvider
{
	private string? _token;

	public void ClearCache()
	{
		_token = null;
	}

	public async Task<string> GetToken()
	{
		if (string.IsNullOrEmpty(_token))
		{
			_token = await userDataStore.GetAccessToken();
		}

		if (string.IsNullOrEmpty(_token))
		{
			throw new SessionUnavailableException();
		}

		var securityToken = new JwtSecurityToken(_token);
		var validTo = securityToken.Payload.ValidTo;

		if (validTo > DateTime.UtcNow.AddMinutes(5))
		{
			return _token!;
		}
		else if (validTo > DateTime.UtcNow)
		{
			using var client = httpClientFactory.CreateClient();
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");
			var response = await client.GetAsync("api/Auth/Refresh").ConfigureAwait(false);

			if (response.IsSuccessStatusCode)
			{
				_token = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				await userDataStore.SetAccessToken(_token);
				return _token;
			}
		}
		else
		{
			var token = await userDataStore.GetAccessToken().ConfigureAwait(false);
			if (!string.IsNullOrWhiteSpace(token) && token != _token)
			{
				_token = token;
				return await GetToken().ConfigureAwait(false);
			}
		}

		throw new SessionUnavailableException();
	}
}