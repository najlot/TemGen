using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Client.MVVM;

namespace <# Project.Namespace#>.ClientBase.Identity;

public class ForgotPasswordViewModel : AbstractValidationViewModel
{
	private readonly IPasswordResetService _passwordResetService;
	private readonly INavigationService _navigationService;
	private readonly INotificationService _notificationService;
	private readonly ILogger<ForgotPasswordViewModel> _logger;

	public AsyncCommand RequestResetCodeCommand { get; }
	public ICommand NavigateToLoginCommand { get; }

	public ForgotPasswordViewModel(
		IPasswordResetService passwordResetService,
		INavigationService navigationService,
		INotificationService notificationService,
		ILogger<ForgotPasswordViewModel> logger)
	{
		_passwordResetService = passwordResetService;
		_navigationService = navigationService;
		_notificationService = notificationService;
		_logger = logger;

		RequestResetCodeCommand = new AsyncCommand(RequestResetCodeAsync, OnError, () => !IsSubmitting && !HasErrors);
		NavigateToLoginCommand = new AsyncCommand(() => _navigationService.NavigateForward<LoginViewModel>(), OnError);

		ErrorsChanged += (s, e) => RequestResetCodeCommand.RaiseCanExecuteChanged();
	}

	public string EMail { get; set => Set(ref field, value); } = string.Empty;
	public bool IsSubmitting { get; private set => Set(ref field, value, RequestResetCodeCommand.RaiseCanExecuteChanged); }

	private async Task RequestResetCodeAsync()
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
			var result = await _passwordResetService.RequestPasswordReset(EMail);
			if (!result.IsSuccess)
			{
				await _notificationService.ShowErrorAsync(result.ErrorMessage);
				return;
			}

			await _notificationService.ClearNotificationsAsync();
			await _navigationService.NavigateForward<ResetPasswordViewModel>(new Dictionary<string, object>
			{
				[nameof(EMail)] = EMail.Trim(),
			});
		}
		finally
		{
			IsSubmitting = false;
		}
	}

	private async Task OnError(Task task)
	{
		var ex = task.Exception;
		_logger.LogError(ex, "An error occurred during password reset request");

		IsSubmitting = false;
		await _notificationService.ShowErrorAsync(ex?.Message ?? ErrorLoc.ErrorCouldNotLoad);
	}

	protected override IEnumerable<ValidationResult> Validate(string? propertyName)
	{
		if (ShouldValidate(propertyName, nameof(EMail)))
		{
			yield return Result(
				nameof(EMail),
				!string.IsNullOrWhiteSpace(EMail) && Regex.IsMatch(EMail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"),
				ErrorLoc.InvalidEmailAddress);
		}
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>