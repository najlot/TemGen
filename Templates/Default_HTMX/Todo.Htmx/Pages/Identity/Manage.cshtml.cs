using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using <# Project.Namespace#>.Client.Data;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Client.Data.Users;

namespace <# Project.Namespace#>.Htmx.Pages.Identity;

[Authorize]
public class ManageModel : PageModel
{
	private readonly IUserService _userService;
	private readonly ITokenProvider _tokenProvider;
	private readonly ILogger<ManageModel> _logger;

	public class InputModel
	{
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
	public bool Saved { get; private set; }

	public ManageModel(IUserService userService, ITokenProvider tokenProvider, ILogger<ManageModel> logger)
	{
		_userService = userService;
		_tokenProvider = tokenProvider;
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

		if (Input.Password != Input.PasswordConfirm)
		{
			ErrorMessage = ErrorLoc.ErrorPasswordsDoNotMatch;
			Input.PasswordConfirm = string.Empty;
			return Page();
		}

		try
		{
			var token = await _tokenProvider.GetToken();

			if (token is null)
			{
				return RedirectToPage("/Identity/Login", new { ReturnUrl = "/Identity/Manage" });
			}

			var user = await _userService.GetCurrentUserAsync();
			user.Password = Input.Password;
			await _userService.UpdateItemAsync(user);

			Input.Password = string.Empty;
			Input.PasswordConfirm = string.Empty;
			Saved = true;
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login", new { ReturnUrl = "/Identity/Manage" });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating user.");
			ErrorMessage = ErrorLoc.ErrorCouldNotUpdateUserData;
		}

		return Page();
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
