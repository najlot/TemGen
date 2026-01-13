using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using <#cs Write(Project.Namespace)#>.Service.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.Service.Services;

public class TokenService
{
	private readonly IUserService _userService;
	private readonly ServiceConfiguration _serviceConfiguration;

	public TokenService(
		IUserService userService,
		ServiceConfiguration serviceConfiguration)
	{
		_userService = userService;
		_serviceConfiguration = serviceConfiguration;
	}

	public static TokenValidationParameters GetValidationParameters(string secret)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

		return new TokenValidationParameters
		{
			ValidateLifetime = true,
			LifetimeValidator = (before, expires, token, param) =>
			{
				return expires > DateTime.UtcNow;
			},
			ValidateAudience = false,
			ValidateIssuer = false,
			ValidateActor = false,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = key
		};
	}

	public TokenValidationParameters GetValidationParameters()
	{
		return GetValidationParameters(_serviceConfiguration.Secret);
	}

	public string GetRefreshToken(string username, Guid userId)
	{
		var claim = new[]
		{
			new Claim(ClaimTypes.Name, username),
			new Claim(ClaimTypes.NameIdentifier, userId.ToString())
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_serviceConfiguration.Secret));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var jwtToken = new JwtSecurityToken(
			issuer: "<#cs Write(Project.Namespace)#>.Service",
			audience: "<#cs Write(Project.Namespace)#>.Service",
			claims: claim,
			expires: DateTime.UtcNow.AddDays(7),
			signingCredentials: credentials
		);

		var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
		return token;
	}

	public async Task<string?> GetToken(string username, string password)
	{
		var user = await _userService.GetUserModelFromName(username).ConfigureAwait(false);

		if (user == null)
		{
			return null;
		}

		var bytes = Encoding.UTF8.GetBytes(password);
		var userUasswordHash = SHA256.HashData(bytes);

		if (!Enumerable.SequenceEqual(user.PasswordHash, userUasswordHash))
		{
			return null;
		}

		var claim = new[]
		{
			new Claim(ClaimTypes.Name, username),
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_serviceConfiguration.Secret));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var jwtToken = new JwtSecurityToken(
			issuer: "<#cs Write(Project.Namespace)#>.Service",
			audience: "<#cs Write(Project.Namespace)#>.Service",
			claims: claim,
			expires: DateTime.UtcNow.AddDays(7),
			signingCredentials: credentials
		);

		var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
		return token;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>