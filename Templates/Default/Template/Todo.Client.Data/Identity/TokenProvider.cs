using Cosei.Client.Base;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Contracts;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Identity;

public class TokenProvider : ITokenProvider
{
	private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
	private readonly Func<IRequestClient> _clientFactory;
	private readonly string _userName;
	private readonly string _password;
	private DateTime _tokenValidTo;
	private string? _token;

	public TokenProvider(Func<IRequestClient> clientFactory, string userName, string password)
	{
		_clientFactory = clientFactory;
		_userName = userName;
		_password = password;
	}

	public async Task<string> GetToken()
	{
		if (_tokenValidTo > DateTime.UtcNow.AddMinutes(5))
		{
			return _token ?? string.Empty;
		}

		await _semaphore.WaitAsync();

		try
		{
			if (_tokenValidTo > DateTime.UtcNow.AddMinutes(5))
			{
				return _token ?? string.Empty;
			}

			using (var client = _clientFactory())
			{
				var request = new AuthRequest
				{
					Username = _userName,
					Password = _password
				};

				var response = await client.PostAsync("api/Auth", JsonSerializer.Serialize(request), "application/json");
				response = response.EnsureSuccessStatusCode();
				_token = Encoding.UTF8.GetString(response.Body.ToArray());
			}

			var securityToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(_token);
			_tokenValidTo = securityToken.Payload.ValidTo;

			return _token ?? string.Empty;
		}
		finally
		{
			_semaphore.Release();
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>