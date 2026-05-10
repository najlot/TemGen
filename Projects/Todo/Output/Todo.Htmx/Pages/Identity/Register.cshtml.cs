using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Todo.Client.Data.Identity;
using Todo.Htmx.Identity;

namespace Todo.Htmx.Pages.Identity;

public class RegisterModel : PageModel
{
	private readonly IRegistrationService _registrationService;
	private readonly ITokenService _tokenService;
	private readonly ILogger<RegisterModel> _logger;

	public class InputModel
	{
		[Required]
		[StringLength(30, MinimumLength = 3)]
		public string Username { get; set; } = string.Empty;

		[Required]
		[EmailAddress]
		public string EMail { get; set; } = string.Empty;

		[Required]
		[DataType(DataType.Password)]
		[StringLength(30, MinimumLength = 8)]
		public string Password { get; set; } = string.Empty;
	}

	[BindProperty]
	public InputModel Input { get; set; } = new();

	public string? ErrorMessage { get; set; }

	public RegisterModel(IRegistrationService registrationService, ITokenService tokenService, ILogger<RegisterModel> logger)
	{
		_registrationService = registrationService;
		_tokenService = tokenService;
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

		var result = await _registrationService.Register(Guid.NewGuid(), Input.Username, Input.EMail, Input.Password);

		if (!result.IsSuccess)
		{
			ErrorMessage = result.ErrorMessage;
			Input.Password = string.Empty;
			return Page();
		}

		string? token;

		try
		{
			token = await _tokenService.CreateToken(Input.Username, Input.Password);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error creating token after registration.");
			return RedirectToPage("/Identity/Login");
		}

		if (string.IsNullOrWhiteSpace(token))
		{
			return RedirectToPage("/Identity/Login");
		}

		await HttpContextUserDataStore.SignInAsync(HttpContext, Input.Username, token);
		return RedirectToPage("/Index");
	}
}

