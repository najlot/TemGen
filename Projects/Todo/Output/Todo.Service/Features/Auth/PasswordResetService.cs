using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Security.Cryptography;
using Todo.Contracts.Auth;
using Todo.Service.Features.Users;
using Todo.Service.Shared.Results;

namespace Todo.Service.Features.Auth;

public class PasswordResetService(
	IUserRepository userRepository,
	IPasswordResetCodeSender passwordResetCodeSender,
	ILogger<PasswordResetService> logger)
{
	private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(15);

	public async Task<Result> RequestPasswordReset(RequestPasswordReset request)
	{
		if (string.IsNullOrWhiteSpace(request.EMail))
		{
			return Result.Validation("Email address is required.");
		}

		if (!passwordResetCodeSender.CanSend)
		{
			return Result.Validation("Password reset is currently unavailable.");
		}

		var user = await userRepository.GetByEmail(request.EMail.Trim()).ConfigureAwait(false);
		if (user is null)
		{
			return Result.Success();
		}

		var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6", CultureInfo.InvariantCulture);
		user.PasswordResetCodeHash = PasswordHasher.HashPassword(code);
		user.PasswordResetCodeExpiresAt = DateTime.UtcNow.Add(CodeLifetime);

		await userRepository.Update(user).ConfigureAwait(false);

		try
		{
			await passwordResetCodeSender.SendAsync(user.EMail, user.Username, code, CodeLifetime, request.Culture).ConfigureAwait(false);
			return Result.Success();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error requesting password reset for {Email}", request.EMail);
			return Result.Validation("Password reset is currently unavailable.");
		}
	}

	public async Task<Result> ResetPassword(ResetPassword request)
	{
		if (string.IsNullOrWhiteSpace(request.EMail))
		{
			return Result.Validation("Email address is required.");
		}

		if (string.IsNullOrWhiteSpace(request.Code))
		{
			return Result.Validation("Reset code is required.");
		}

		if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Trim().Length < 6)
		{
			return Result.Validation("Password too short!");
		}

		var user = await userRepository.GetByEmail(request.EMail.Trim()).ConfigureAwait(false);
		if (user is null
			|| user.PasswordResetCodeHash is not { Length: > 0 }
			|| user.PasswordResetCodeExpiresAt is null
			|| user.PasswordResetCodeExpiresAt <= DateTime.UtcNow
			|| !PasswordHasher.VerifyPassword(request.Code.Trim(), user.PasswordResetCodeHash))
		{
			return Result.Validation("Invalid or expired password reset code.");
		}

		user.PasswordHash = PasswordHasher.HashPassword(request.Password);
		user.PasswordResetCodeHash = null;
		user.PasswordResetCodeExpiresAt = null;

		await userRepository.Update(user).ConfigureAwait(false);
		return Result.Success();
	}
}