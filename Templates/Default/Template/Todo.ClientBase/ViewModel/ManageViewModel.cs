using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.Data.Services;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Client.MVVM;

namespace <# Project.Namespace#>.ClientBase.ViewModel;

public class ManageViewModel : AbstractValidationViewModel
{
	private readonly IUserService _userService;
	private readonly ITokenProvider _tokenProvider;
	private readonly INavigationService _navigationService;
	private readonly INotificationService _notificationService;
	private readonly ILogger<ManageViewModel> _logger;

	public AsyncCommand SaveCommand { get; }
	public AsyncCommand NavigateBackCommand { get; }

	public ManageViewModel(
		IUserService userService,
		ITokenProvider tokenProvider,
		INavigationService navigationService,
		INotificationService notificationService,
		ILogger<ManageViewModel> logger)
	{
		_userService = userService;
		_tokenProvider = tokenProvider;
		_navigationService = navigationService;
		_notificationService = notificationService;
		_logger = logger;

		SaveCommand = new AsyncCommand(SavePasswordAsync, OnError, () => !IsSubmitting && !HasErrors);
		NavigateBackCommand = new AsyncCommand(() => _navigationService.NavigateBack(), OnError);

		ErrorsChanged += (s, e) => SaveCommand.RaiseCanExecuteChanged();
	}

	public string Password { get; set => Set(ref field, value); } = string.Empty;
	public string PasswordConfirm { get; set => Set(ref field, value); } = string.Empty;
	public bool IsSubmitting { get; private set => Set(ref field, value, SaveCommand.RaiseCanExecuteChanged); }

	private async Task SavePasswordAsync()
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
			var token = await _tokenProvider.GetToken();
			if (string.IsNullOrWhiteSpace(token))
			{
				await _navigationService.NavigateForward<LoginViewModel>();
				return;
			}

			var user = await _userService.GetCurrentUserAsync();
			user.Password = Password;
			await _userService.UpdateItemAsync(user);

			Password = string.Empty;
			PasswordConfirm = string.Empty;

			await _notificationService.ShowSuccessAsync(UserLoc.ChangePassword);
		}
		catch (System.Exception ex)
		{
			_logger.LogError(ex, "Error updating user password");
			await _notificationService.ShowErrorAsync(ex.Message ?? ErrorLoc.ErrorCouldNotLoad);
		}
		finally
		{
			IsSubmitting = false;
		}
	}

	private async Task OnError(Task task)
	{
		var ex = task.Exception;
		_logger.LogError(ex, "An error occurred during password change");

		IsSubmitting = false;
		await _notificationService.ShowErrorAsync(ex?.Message ?? ErrorLoc.ErrorCouldNotLoad);
	}

	protected override IEnumerable<ValidationResult> Validate(string? propertyName)
	{
		yield return Result(
			nameof(Password),
			!string.IsNullOrWhiteSpace(Password) && Password.Length >= 6,
			ErrorLoc.PasswordTooShort);

		yield return Result(nameof(PasswordConfirm), Password == PasswordConfirm, ErrorLoc.ErrorPasswordsDoNotMatch);
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>