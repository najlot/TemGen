using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Todo.Service.Features.Users;
using Todo.Service.Shared.Configuration;

namespace Todo.Service.Features.Auth;

public class TokenService(
	IUserService userService,
	IUserIdProvider userIdProvider,
	ServiceConfiguration serviceConfiguration)
{
	private const string TokenIssuer = "Todo.Service";
	private const string TokenAudience = TokenIssuer;

	public static TokenValidationParameters GetValidationParameters(string secret)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

		return new TokenValidationParameters
		{
			RequireSignedTokens = true,
			RequireExpirationTime = true,
			ValidateLifetime = true,
			ClockSkew = TimeSpan.Zero,
			ValidateAudience = true,
			ValidAudience = TokenAudience,
			ValidateIssuer = true,
			ValidIssuer = TokenIssuer,
			ValidateActor = false,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = key
		};
	}

	public TokenValidationParameters GetValidationParameters()
	{
		return GetValidationParameters(serviceConfiguration.Secret);
	}

	public string GetRefreshToken(string username)
	{
		var userId = userIdProvider.GetRequiredUserId();
		var claim = new[]
		{
			new Claim(ClaimTypes.Name, username),
			new Claim(ClaimTypes.NameIdentifier, userId.ToString())
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(serviceConfiguration.Secret));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var jwtToken = new JwtSecurityToken(
			issuer: TokenIssuer,
			audience: TokenAudience,
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

		if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
		{
			return null;
		}

		var claim = new[]
		{
			new Claim(ClaimTypes.Name, user.Username),
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(serviceConfiguration.Secret));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var jwtToken = new JwtSecurityToken(
			issuer: TokenIssuer,
			audience: TokenAudience,
			claims: claim,
			expires: DateTime.UtcNow.AddDays(7),
			signingCredentials: credentials
		);

		var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
		return token;
	}
}