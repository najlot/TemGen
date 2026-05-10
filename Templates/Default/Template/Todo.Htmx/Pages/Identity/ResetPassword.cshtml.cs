using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.Localisation;

namespace <# Project.Namespace#>.Htmx.Pages.Identity;

public class ResetPasswordModel : PageModel
{
	private readonly IPasswordResetService _passwordResetService;
	private readonly ILogger<ResetPasswordModel> _logger;

	public class InputModel
	{
		public string EMail { get; set; } = string.Empty;

		[Required]
		public string Code { get; set; } = string.Empty;

		[Required]
		[DataType(DataType.Password)]
		[StringLength(30, MinimumLength = 8)]
		public string Password { get; set; } = string.Empty;

		[Required]
		[DataType(DataType.Password)]
		[StringLength(30, MinimumLength = 8)]
		public string PasswordConfirm { get; set; } = string.Empty;
	}

	[BindProperty]
	public InputModel Input { get; set; } = new();

	public string? ErrorMessage { get; set; }

	public ResetPasswordModel(IPasswordResetService passwordResetService, ILogger<ResetPasswordModel> logger)
	{
		_passwordResetService = passwordResetService;
		_logger = logger;
	}

	public IActionResult OnGet([FromQuery] string? email)
	{
		if (string.IsNullOrWhiteSpace(email))
		{
			return RedirectToPage("/Identity/ForgotPassword");
		}

		Input.EMail = email;
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (string.IsNullOrWhiteSpace(Input.EMail))
		{
			return RedirectToPage("/Identity/ForgotPassword");
		}

		if (!ModelState.IsValid)
		{
			ErrorMessage = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
			return Page();
		}

		if (Input.Password != Input.PasswordConfirm)
		{
			ErrorMessage = ErrorLoc.ErrorPasswordsDoNotMatch;
			Input.PasswordConfirm = string.Empty;
			return Page();
		}

		try
		{
			var result = await _passwordResetService.ResetPassword(Input.EMail, Input.Code, Input.Password);

			if (!result.IsSuccess)
			{
				ErrorMessage = result.ErrorMessage;
				Input.Code = string.Empty;
				return Page();
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error resetting password.");
			ErrorMessage = ErrorLoc.ErrorCouldNotSave;
			return Page();
		}

		return RedirectToPage("/Identity/Login");
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
