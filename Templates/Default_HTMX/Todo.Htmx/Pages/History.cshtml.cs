using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using <# Project.Namespace#>.Client.Data.History;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Contracts.History;

namespace <# Project.Namespace#>.Htmx.Pages;

[Authorize]
public class HistoryModel : PageModel
{
	private readonly IHistoryService _historyService;
	private readonly ILogger<HistoryModel> _log;

	public bool IsLoading { get; private set; }
	public HistoryEntry[] Entries { get; private set; } = [];

	public HistoryModel(IHistoryService historyService, ILogger<HistoryModel> log)
	{
		_historyService = historyService;
		_log = log;
	}

	public async Task<IActionResult> OnGetAsync(Guid id, string? entityName)
	{
		if (id == Guid.Empty)
		{
			return Page();
		}

		try
		{
			IsLoading = true;
			Entries = await _historyService.GetItemsAsync(id, entityName ?? string.Empty);
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error loading history for entity {EntityId}.", id);
			TempData["ErrorTitle"] = "History";
			TempData["ErrorMessage"] = ex.Message;
		}
		finally
		{
			IsLoading = false;
		}

		return Page();
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
