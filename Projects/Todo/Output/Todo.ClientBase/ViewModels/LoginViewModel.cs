using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Todo.Client.Data.Identity;
using Todo.Client.Localisation;
using Todo.Client.MVVM;

namespace Todo.ClientBase.ViewModels;

public class LoginViewModel : AbstractValidationViewModel, IAsyncInitializable, ISessionStart
{
	private readonly ITokenService _tokenService;
	private readonly IUserDataStore _userDataStore;
	private readonly ITokenProvider _tokenProvider;
	private readonly INavigationService _navigationService;
	private readonly INotificationService _notificationService;
	private readonly ILogger<LoginViewModel> _logger;

	public AsyncCommand LoginCommand { get; }
	public ICommand NavigateToRegisterCommand { get; }

	public LoginViewModel(
		ITokenService tokenService,
		IUserDataStore userDataStore,
		ITokenProvider tokenProvider,
		INavigationService navigationService,
		INotificationService notificationService,
		ILogger<LoginViewModel> logger)
	{
		_tokenService = tokenService;
		_userDataStore = userDataStore;
		_tokenProvider = tokenProvider;
		_navigationService = navigationService;
		_notificationService = notificationService;
		_logger = logger;

		LoginCommand = new AsyncCommand(TryLoginAsync, OnError, () => !IsSubmitting && !HasErrors);
		NavigateToRegisterCommand = new AsyncCommand(() => _navigationService.NavigateForward<RegisterViewModel>(), OnError);

		ErrorsChanged += (s, e) => LoginCommand.RaiseCanExecuteChanged();
	}

	public string Username { get; set => Set(ref field, value); } = string.Empty;
	public string Password { get; set => Set(ref field, value); } = string.Empty;
	public bool IsSubmitting { get; private set => Set(ref field, value, LoginCommand.RaiseCanExecuteChanged); }

	public Task InitializeAsync()
	{
		// Clear any cached token on initialization
		_tokenProvider.ClearCache();
		return Task.CompletedTask;
	}

	private async Task TryLoginAsync()
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
			string? token;
			try
			{
				token = await _tokenService.CreateToken(Username, Password);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to create token for user {Username}", Username);
				await _notificationService.ShowErrorAsync(ex.Message ?? ErrorLoc.ErrorCouldNotLoad);
				return;
			}

			if (string.IsNullOrWhiteSpace(token))
			{
				_logger.LogWarning("Login failed for user {Username}: invalid credentials", Username);
				await _notificationService.ShowErrorAsync(ErrorLoc.ErrorInvalidUsernamePassword);
			}
			else
			{
				await _userDataStore.SetUserData(Username, token);
				await _notificationService.ClearNotificationsAsync();

				await _navigationService.NavigateForward<MenuViewModel>();
			}

			Password = string.Empty;
		}
		finally
		{
			IsSubmitting = false;
		}
	}

	private async Task OnError(Task task)
	{
		var ex = task.Exception;
		_logger.LogError(ex, "An error occurred during login operation");

		IsSubmitting = false;
		await _notificationService.ShowErrorAsync(ex?.Message ?? ErrorLoc.ErrorCouldNotLoad);
	}

	protected override IEnumerable<ValidationResult> Validate(string? propertyName)
	{
		yield return Result(
			nameof(Username),
			!string.IsNullOrWhiteSpace(Username) && Username.Length >= 3,
			ErrorLoc.UsernameTooShort);

		yield return Result(
			nameof(Password),
			!string.IsNullOrWhiteSpace(Password) && Password.Length >= 6,
			ErrorLoc.PasswordTooShort);
	}
}