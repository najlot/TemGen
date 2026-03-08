using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Contracts.Commands;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Identity;

public class RegistrationService(IHttpClientFactory httpClientFactory, ILogger<RegistrationService> log)
	: IRegistrationService
{
	public async Task<RegistrationResult> Register(Guid id, string username, string email, string password)
	{
		try
		{
			log.LogDebug("Registering user.");
			using var client = httpClientFactory.CreateClient();

			var command = new CreateUser(
				id,
				username,
				email,
				password);

			var result = await client.PostAsJsonAsync("api/Auth/Register", command);

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
}<#cs SetOutputPathAndSkipOtherDefinitions()#>