using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Todo.Client.Data;
using Todo.Contracts.Users;

namespace Todo.Client.Data.Identity;

public class RegistrationService(IHttpClientFactory httpClientFactory, ILogger<RegistrationService> log)
	: IRegistrationService
{
	public async Task<RegistrationResult> Register(Guid id, string username, string email, string password)
	{
		try
		{
			log.LogDebug("Registering user.");
			using var client = httpClientFactory.CreateClient();

			var command = new CreateUser
			{
				Id = id,
				Username = username,
				EMail = email,
				Password = password
			};

			var result = await client.PostAsJsonAsync("api/Auth/Register", command, ClientDataJsonSerializer.Options);

			if (result.IsSuccessStatusCode)
			{
				return RegistrationResult.Success();
			}

			var errorText = await result.Content.ReadAsStringAsync();
			return RegistrationResult.Failure($"{result.StatusCode} {errorText}");
		}
		catch (Exception ex)
		{
			log.LogError(ex, "Error registering user.");
			return RegistrationResult.Failure("Registration is currently unavailable. Please try later.");
		}
	}
}