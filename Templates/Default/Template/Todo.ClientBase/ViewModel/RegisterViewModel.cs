using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using <#cs Write(Project.Namespace)#>.Client.Data.Identity;
using <#cs Write(Project.Namespace)#>.Client.Localisation;
using <#cs Write(Project.Namespace)#>.Client.MVVM;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

public class RegisterViewModel : AbstractValidationViewModel, IAsyncInitializable
{
	private readonly IRegistrationService _registrationService;
	private readonly ITokenService _tokenService;
	private readonly IUserDataStore _userDataStore;
	private readonly ITokenProvider _tokenProvider;
	private readonly INavigationService _navigationService;
	private readonly INotificationService _notificationService;
	private readonly ILogger<RegisterViewModel> _logger;

	public AsyncCommand RegisterCommand { get; }
	public ICommand NavigateToLoginCommand { get; }

	public RegisterViewModel(
		IRegistrationService registrationService,
		ITokenService tokenService,
		IUserDataStore userDataStore,
		ITokenProvider tokenProvider,
		INavigationService navigationService,
		INotificationService notificationService,
		ILogger<RegisterViewModel> logger)
	{
		_registrationService = registrationService;
		_tokenService = tokenService;
		_userDataStore = userDataStore;
		_tokenProvider = tokenProvider;
		_navigationService = navigationService;
		_notificationService = notificationService;
		_logger = logger;

		RegisterCommand = new AsyncCommand(TryRegisterAsync, OnError, () => !IsSubmitting && !HasErrors);
		NavigateToLoginCommand = new AsyncCommand(() => _navigationService.NavigateBack(), OnError);

		ErrorsChanged += (s, e) => RegisterCommand.RaiseCanExecuteChanged();
	}

	public string Username { get; set => Set(ref field, value); } = string.Empty;
	public string EMail { get; set => Set(ref field, value); } = string.Empty;
	public string Password { get; set => Set(ref field, value); } = string.Empty;
	public bool IsSubmitting { get; private set => Set(ref field, value, RegisterCommand.RaiseCanExecuteChanged); }

	public Task InitializeAsync()
	{
		_tokenProvider.ClearCache();
		return Task.CompletedTask;
	}

	private async Task TryRegisterAsync()
	{
		ValidateAll();

		if (HasErrors)
		{
			return;
		}

		IsSubmitting = true;
		await _notificationService.ClearNotificationsAsync();

		try
		{
			var result = await _registrationService.Register(Guid.NewGuid(), Username, EMail, Password);

			if (!result.IsSuccess)
			{
				Password = string.Empty;
				await _notificationService.ShowErrorAsync(result.ErrorMessage);
				return;
			}

			string? token;
			try
			{
				token = await _tokenService.CreateToken(Username, Password);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to create token for user {Username}", Username);
				await _notificationService.ShowErrorAsync(ex.Message ?? CommonLoc.ErrorCouldNotLoad);
				return;
			}

			if (string.IsNullOrWhiteSpace(token))
			{
				_logger.LogWarning("Auto-login failed for user {Username} after registration", Username);
				await _notificationService.ShowErrorAsync(CommonLoc.ErrorCouldNotLoad);
			}
			else
			{
				await _userDataStore.SetUserData(Username, token);
				await _notificationService.ClearNotificationsAsync();
				await _navigationService.NavigateForward<MenuViewModel>();
			}
		}
		finally
		{
			IsSubmitting = false;
		}
	}

	private async Task OnError(Task task)
	{
		var ex = task.Exception;
		_logger.LogError(ex, "An error occurred during registration operation");

		IsSubmitting = false;
		await _notificationService.ShowErrorAsync(ex?.Message ?? CommonLoc.ErrorCouldNotLoad);
	}

	protected override IEnumerable<ValidationResult> Validate(string? propertyName)
	{
		yield return Result(
			nameof(Username),
			!string.IsNullOrWhiteSpace(Username) && Username.Length >= 3,
			"Username too short");

		if (ShouldValidate(propertyName, nameof(EMail)))
		{
			yield return Result(
				nameof(EMail),
				!string.IsNullOrWhiteSpace(EMail) && Regex.IsMatch(EMail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"),
				"Invalid email address");
		}

		yield return Result(
			nameof(Password),
			!string.IsNullOrWhiteSpace(Password) && Password.Length >= 6,
			"Password too short");
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
