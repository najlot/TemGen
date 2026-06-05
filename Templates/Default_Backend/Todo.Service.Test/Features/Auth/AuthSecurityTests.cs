using System;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using <# Project.Namespace#>.Service.Features.Auth;

namespace <# Project.Namespace#>.Service.Test.Features.Auth;

public class AuthSecurityTests
{
	private const string Secret = "12345678901234567890123456789012";

	[Test]
	public void HashPassword_must_roundtrip()
	{
		const string password = "correct horse battery staple";

		var passwordHash = PasswordHasher.HashPassword(password);

		Assert.Multiple(() =>
		{
			Assert.That(passwordHash, Has.Length.GreaterThan(32));
			Assert.That(PasswordHasher.VerifyPassword(password, passwordHash), Is.True);
			Assert.That(PasswordHasher.VerifyPassword("wrong password", passwordHash), Is.False);
		});
	}

	[Test]
	public void VerifyPassword_must_accept_legacy_sha256_hashes()
	{
		const string password = "legacy-password";
		var legacyHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));

		Assert.Multiple(() =>
		{
			Assert.That(PasswordHasher.VerifyPassword(password, legacyHash), Is.True);
			Assert.That(PasswordHasher.VerifyPassword("wrong password", legacyHash), Is.False);
		});
	}

	[Test]
	public void ValidationParameters_must_accept_tokens_with_expected_issuer_and_audience()
	{
		var handler = new JwtSecurityTokenHandler();
		var credentials = new SigningCredentials(
			new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
			SecurityAlgorithms.HmacSha256);
		var token = handler.WriteToken(new JwtSecurityToken(
			issuer: "<# Project.Namespace#>.Service",
			audience: "<# Project.Namespace#>.Service",
			claims: [new Claim(ClaimTypes.Name, "alice")],
			expires: DateTime.UtcNow.AddMinutes(5),
			signingCredentials: credentials));

		var principal = handler.ValidateToken(token, TokenService.GetValidationParameters(Secret), out _);

		Assert.That(principal.Identity?.Name, Is.EqualTo("alice"));
	}

	[Test]
	public void ValidationParameters_must_reject_unexpected_audience()
	{
		var handler = new JwtSecurityTokenHandler();
		var credentials = new SigningCredentials(
			new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
			SecurityAlgorithms.HmacSha256);
		var token = handler.WriteToken(new JwtSecurityToken(
			issuer: "<# Project.Namespace#>.Service",
			audience: "unexpected-audience",
			claims: [new Claim(ClaimTypes.Name, "alice")],
			expires: DateTime.UtcNow.AddMinutes(5),
			signingCredentials: credentials));

		Assert.Throws<SecurityTokenInvalidAudienceException>(
			() => handler.ValidateToken(token, TokenService.GetValidationParameters(Secret), out _));
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>