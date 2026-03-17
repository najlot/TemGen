using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using <# Project.Namespace#>.Service.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace <# Project.Namespace#>.Service.Services;

public class TokenService(
	IUserService userService,
	ServiceConfiguration serviceConfiguration)
{
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
		return GetValidationParameters(serviceConfiguration.Secret);
	}

	public string GetRefreshToken(string username, Guid userId)
	{
		var claim = new[]
		{
			new Claim(ClaimTypes.Name, username),
			new Claim(ClaimTypes.NameIdentifier, userId.ToString())
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(serviceConfiguration.Secret));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var jwtToken = new JwtSecurityToken(
			issuer: "<# Project.Namespace#>.Service",
			audience: "<# Project.Namespace#>.Service",
			claims: claim,
			expires: DateTime.UtcNow.AddDays(7),
			signingCredentials: credentials
		);

		var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
		return token;
	}

	public async Task<string?> GetToken(string username, string password)
	{
		var user = await userService.GetUserModelFromName(username).ConfigureAwait(false);

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

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(serviceConfiguration.Secret));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var jwtToken = new JwtSecurityToken(
			issuer: "<# Project.Namespace#>.Service",
			audience: "<# Project.Namespace#>.Service",
			claims: claim,
			expires: DateTime.UtcNow.AddDays(7),
			signingCredentials: credentials
		);

		var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
		return token;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>