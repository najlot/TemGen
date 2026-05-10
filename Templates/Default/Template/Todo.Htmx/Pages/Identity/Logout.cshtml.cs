using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace <# Project.Namespace#>.Htmx.Pages.Identity;

public class LogoutModel : PageModel
{
	public async Task<IActionResult> OnPostAsync()
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		return RedirectToPage("/Index");
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
