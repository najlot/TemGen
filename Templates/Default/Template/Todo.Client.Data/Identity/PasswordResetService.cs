using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data;
using <# Project.Namespace#>.Contracts.Auth;

namespace <# Project.Namespace#>.Client.Data.Identity;

public class PasswordResetService(IHttpClientFactory httpClientFactory, ILogger<PasswordResetService> log)
	: IPasswordResetService
{
	public async Task<PasswordResetResult> RequestPasswordReset(string email)
	{
		try
		{
			log.LogDebug("Requesting password reset code.");
			using var client = httpClientFactory.CreateClient();

			var command = new RequestPasswordReset
			{
				EMail = email,
				Culture = CultureInfo.CurrentUICulture.Name,
			};

			var result = await client.PostAsJsonAsync("api/Auth/RequestPasswordReset", command, ClientDataJsonSerializer.Options);

			if (result.IsSuccessStatusCode)
			{
				return PasswordResetResult.Success();
			}

			var errorText = await result.Content.ReadAsStringAsync();
			return PasswordResetResult.Failure(string.IsNullOrWhiteSpace(errorText)
				? "Password reset is currently unavailable. Please try later."
				: errorText);
		}
		catch (Exception ex)
		{
			log.LogError(ex, "Error requesting password reset code.");
			return PasswordResetResult.Failure("Password reset is currently unavailable. Please try later.");
		}
	}

	public async Task<PasswordResetResult> ResetPassword(string email, string code, string password)
	{
		try
		{
			log.LogDebug("Resetting password.");
			using var client = httpClientFactory.CreateClient();

			var command = new ResetPassword
			{
				EMail = email,
				Code = code,
				Password = password
			};

			var result = await client.PostAsJsonAsync("api/Auth/ResetPassword", command, ClientDataJsonSerializer.Options);

			if (result.IsSuccessStatusCode)
			{
				return PasswordResetResult.Success();
			}

			var errorText = await result.Content.ReadAsStringAsync();
			return PasswordResetResult.Failure(string.IsNullOrWhiteSpace(errorText)
				? "Password reset is currently unavailable. Please try later."
				: errorText);
		}
		catch (Exception ex)
		{
			log.LogError(ex, "Error resetting password.");
			return PasswordResetResult.Failure("Password reset is currently unavailable. Please try later.");
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>