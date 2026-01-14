using Cosei.Client.Base;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Client.Data.Identity;

public class RefreshingTokenProvider : ITokenProvider
{
	private readonly IRequestClient _client;
	private readonly IUserDataStore _userDataStore;
	private string? _token;

	public RefreshingTokenProvider(IRequestClient client, IUserDataStore userDataStore)
	{
		_client = client;
		_userDataStore = userDataStore;
	}

	public async Task<string> GetToken()
	{
		if (string.IsNullOrEmpty(_token))
		{
			_token = await _userDataStore.GetAccessToken();
		}

		if (string.IsNullOrEmpty(_token))
		{
			throw new AuthenticationException();
		}

		var securityToken = new JwtSecurityToken(_token);
		var validTo = securityToken.Payload.ValidTo;

		if (validTo > DateTime.UtcNow.AddMinutes(5))
		{
			return _token!;
		}
		else if (validTo > DateTime.UtcNow)
		{
			var headers = new Dictionary<string, string>
			{
				{ "Authorization", $"Bearer {_token}" }
			};

			var response = await _client.GetAsync("api/Auth/Refresh", headers);

			if (response.StatusCode >= 200 && response.StatusCode < 300)
			{
				_token = Encoding.UTF8.GetString(response.Body.ToArray());
				await _userDataStore.SetAccessToken(_token);
				return _token;
			}
		}
		else
		{
			var token = await _userDataStore.GetAccessToken();
			if (!string.IsNullOrWhiteSpace(token) && token != _token)
			{
				_token = token;
				return await GetToken();
			}
		}

		throw new AuthenticationException();
	}
}