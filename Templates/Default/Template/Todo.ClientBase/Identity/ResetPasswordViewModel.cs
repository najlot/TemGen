using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Client.MVVM;

namespace <# Project.Namespace#>.ClientBase.Identity;

public class ResetPasswordViewModel : AbstractValidationViewModel, IParameterizable
{
	private readonly IPasswordResetService _passwordResetService;
	private readonly INavigationService _navigationService;
	private readonly INotificationService _notificationService;
	private readonly ILogger<ResetPasswordViewModel> _logger;

	public AsyncCommand ResetPasswordCommand { get; }
	public AsyncCommand NavigateBackCommand { get; }

	public ResetPasswordViewModel(
		IPasswordResetService passwordResetService,
		INavigationService navigationService,
		INotificationService notificationService,
		ILogger<ResetPasswordViewModel> logger)
	{
		_passwordResetService = passwordResetService;
		_navigationService = navigationService;
		_notificationService = notificationService;
		_logger = logger;

		ResetPasswordCommand = new AsyncCommand(TryResetPasswordAsync, OnError, () => !IsSubmitting && !HasErrors);
		NavigateBackCommand = new AsyncCommand(() => _navigationService.NavigateBack(), OnError);

		ErrorsChanged += (s, e) => ResetPasswordCommand.RaiseCanExecuteChanged();
	}

	public string EMail { get; private set => Set(ref field, value); } = string.Empty;
	public string Code { get; set => Set(ref field, value); } = string.Empty;
	public string Password { get; set => Set(ref field, value); } = string.Empty;
	public string PasswordConfirm { get; set => Set(ref field, value); } = string.Empty;
	public bool IsSubmitting { get; private set => Set(ref field, value, ResetPasswordCommand.RaiseCanExecuteChanged); }

	public void SetParameters(IReadOnlyDictionary<string, object> parameters)
	{
		if (parameters.TryGetValue(nameof(EMail), out var emailObj) && emailObj is string email)
		{
			EMail = email;
		}
	}

	private async Task TryResetPasswordAsync()
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
			var result = await _passwordResetService.ResetPassword(EMail, Code, Password);
			if (!result.IsSuccess)
			{
				Code = string.Empty;
				await _notificationService.ShowErrorAsync(result.ErrorMessage);
				return;
			}

			Code = string.Empty;
			Password = string.Empty;
			PasswordConfirm = string.Empty;

			await _notificationService.ClearNotificationsAsync();
			await _navigationService.NavigateForward<LoginViewModel>();
		}
		finally
		{
			IsSubmitting = false;
		}
	}

	private async Task OnError(Task task)
	{
		var ex = task.Exception;
		_logger.LogError(ex, "An error occurred during password reset");

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

		yield return Result(nameof(Code), !string.IsNullOrWhiteSpace(Code), ErrorLoc.ResetCodeRequired);

		yield return Result(
			nameof(Password),
			!string.IsNullOrWhiteSpace(Password) && Password.Length >= 6,
			ErrorLoc.PasswordTooShort);

		yield return Result(nameof(PasswordConfirm), Password == PasswordConfirm, ErrorLoc.ErrorPasswordsDoNotMatch);
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>