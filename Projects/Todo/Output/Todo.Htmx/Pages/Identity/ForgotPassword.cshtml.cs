using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Todo.Client.Data.Identity;
using Todo.Client.Localisation;

namespace Todo.Htmx.Pages.Identity;

public class ForgotPasswordModel : PageModel
{
	private readonly IPasswordResetService _passwordResetService;
	private readonly ILogger<ForgotPasswordModel> _logger;

	public class InputModel
	{
		[Required]
		[EmailAddress]
		public string EMail { get; set; } = string.Empty;
	}

	[BindProperty]
	public InputModel Input { get; set; } = new();

	public string? ErrorMessage { get; set; }

	public ForgotPasswordModel(IPasswordResetService passwordResetService, ILogger<ForgotPasswordModel> logger)
	{
		_passwordResetService = passwordResetService;
		_logger = logger;
	}

	public void OnGet()
	{
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
		{
			ErrorMessage = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
			return Page();
		}

		try
		{
			var result = await _passwordResetService.RequestPasswordReset(Input.EMail);

			if (!result.IsSuccess)
			{
				ErrorMessage = result.ErrorMessage;
				return Page();
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error requesting password reset.");
			ErrorMessage = ErrorLoc.ErrorCouldNotLoad;
			return Page();
		}

		return RedirectToPage("/Identity/ResetPassword", new { email = Input.EMail.Trim() });
	}
}

