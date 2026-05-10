using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Todo.Client.Data.Identity;
using Todo.Client.Localisation;
using Todo.Htmx.Identity;

namespace Todo.Htmx.Pages.Identity;

public class LoginModel : PageModel
{
	private readonly ITokenService _tokenService;
	private readonly ILogger<LoginModel> _logger;

	public class InputModel
	{
		[Required]
		[StringLength(30, MinimumLength = 3)]
		public string Username { get; set; } = string.Empty;

		[Required]
		[DataType(DataType.Password)]
		[StringLength(30, MinimumLength = 8)]
		public string Password { get; set; } = string.Empty;
	}

	[BindProperty]
	public InputModel Input { get; set; } = new();

	public string? ErrorMessage { get; set; }

	public LoginModel(ITokenService tokenService, ILogger<LoginModel> logger)
	{
		_tokenService = tokenService;
		_logger = logger;
	}

	public void OnGet()
	{
	}

	public async Task<IActionResult> OnPostAsync([FromQuery] string? returnUrl)
	{
		if (!ModelState.IsValid)
		{
			ErrorMessage = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
			return Page();
		}

		string? token;

		try
		{
			token = await _tokenService.CreateToken(Input.Username, Input.Password);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error creating token.");
			ErrorMessage = ErrorLoc.ErrorCouldNotLogin;
			return Page();
		}

		if (string.IsNullOrWhiteSpace(token))
		{
			ErrorMessage = ErrorLoc.ErrorInvalidUsernamePassword;
			Input.Password = string.Empty;
			return Page();
		}

		await HttpContextUserDataStore.SignInAsync(HttpContext, Input.Username, token);

		if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
		{
			return Redirect(returnUrl);
		}

		return RedirectToPage("/Index");
	}
}

