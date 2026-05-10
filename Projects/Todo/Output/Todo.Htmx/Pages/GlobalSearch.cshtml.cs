using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Todo.Client.Data.GlobalSearch;
using Todo.Client.Data.Identity;
using Todo.Contracts.Shared;

namespace Todo.Htmx.Pages;

[Authorize]
public class GlobalSearchModel : PageModel
{
	private readonly IGlobalSearchService _globalSearchService;
	private readonly ILogger<GlobalSearchModel> _log;

	public string Query { get; private set; } = string.Empty;
	public List<GlobalSearchItemModel> Items { get; private set; } = [];
	public bool IsLoaded { get; private set; }

	public GlobalSearchModel(IGlobalSearchService globalSearchService, ILogger<GlobalSearchModel> log)
	{
		_globalSearchService = globalSearchService;
		_log = log;
	}

	public async Task<IActionResult> OnGetAsync([FromQuery] string? q)
	{
		Query = q?.Trim() ?? string.Empty;

		if (string.IsNullOrWhiteSpace(Query))
		{
			return Page();
		}

		try
		{
			Items.AddRange(await _globalSearchService.SearchAsync(Query).ConfigureAwait(false));
			IsLoaded = true;
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login", new { ReturnUrl = "/global-search" });
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error during global search.");
			TempData["ErrorTitle"] = "Search";
			TempData["ErrorMessage"] = ex.Message;
		}

		if (Request.Headers.ContainsKey("HX-Request"))
		{
			return Partial("_GlobalSearchResults", this);
		}

		return Page();
	}

	public static string GetResultHref(GlobalSearchItemModel item)
	{
		return item.Type switch
		{
			ItemType.Note => "/note/" + item.Id,
			ItemType.TodoItem => "/todoitem/" + item.Id,
			_ => "/",
		};
	}
}

