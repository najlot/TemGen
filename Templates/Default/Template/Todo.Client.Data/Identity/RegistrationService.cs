using Cosei.Client.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Contracts.Commands;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Identity;

public class RegistrationService : IRegistrationService
{
	private readonly IRequestClient _requestClient;
	private readonly ILogger _logger;

	public RegistrationService(IRequestClient requestClient, ILogger<RegistrationService> logger)
	{
		_requestClient = requestClient;
		_logger = logger;
	}

	public async Task<RegistrationResult> Register(Guid id, string username, string email, string password)
	{
		try
		{
			_logger.LogDebug("Registering user.");

			var command = new CreateUser(
				id,
				username,
				email,
				password);

			var json = System.Text.Json.JsonSerializer.Serialize(command);
			var result = await _requestClient.PostAsync("api/Auth/Register", json, "application/json");

			if (result.StatusCode >= 200 && result.StatusCode < 300)
			{
				return RegistrationResult.Success();
			}

			return RegistrationResult.Failure(result.StatusCode.ToString() + " " + Encoding.UTF8.GetString(result.Body.ToArray()));
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error registering user.");

			return RegistrationResult.Failure("Registration is currently unavailable. Please try later.");
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>