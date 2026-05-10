using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Todo.Client.Data.Identity;
using Todo.Client.Data.Trash;
using Todo.Client.Localisation;
using Todo.Contracts.Shared;
using Todo.Contracts.Trash;

namespace Todo.Htmx.Pages;

[Authorize]
public class TrashModel : PageModel
{
	private readonly ITrashService _trashService;
	private readonly ILogger<TrashModel> _log;

	public List<TrashItemModel> Items { get; private set; } = [];

	public TrashModel(ITrashService trashService, ILogger<TrashModel> log)
	{
		_trashService = trashService;
		_log = log;
	}

	public async Task<IActionResult> OnGetAsync()
	{
		try
		{
			Items = [.. await _trashService.GetItemsAsync().ConfigureAwait(false)];
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error loading trash.");
			TempData["ErrorTitle"] = TrashLoc.Trash;
			TempData["ErrorMessage"] = ex.Message;
		}

		return Page();
	}

	public async Task<IActionResult> OnPostRestoreAsync([FromForm] string type, [FromForm] Guid id)
	{
		try
		{
			await _trashService.RestoreItemAsync(Enum.Parse<ItemType>(type), id).ConfigureAwait(false);
			Items = [.. await _trashService.GetItemsAsync().ConfigureAwait(false)];
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error restoring trash item.");
			TempData["ErrorTitle"] = TrashLoc.Restore;
			TempData["ErrorMessage"] = ex.Message;
		}

		if (Request.Headers.ContainsKey("HX-Request"))
		{
			return Partial("_TrashList", this);
		}

		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostDeleteAsync([FromForm] string type, [FromForm] Guid id)
	{
		try
		{
			await _trashService.DeleteItemAsync(Enum.Parse<ItemType>(type), id).ConfigureAwait(false);
			Items = [.. await _trashService.GetItemsAsync().ConfigureAwait(false)];
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error deleting trash item.");
			TempData["ErrorTitle"] = "Delete";
			TempData["ErrorMessage"] = ex.Message;
		}

		if (Request.Headers.ContainsKey("HX-Request"))
		{
			return Partial("_TrashList", this);
		}

		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostDeleteAllAsync()
	{
		try
		{
			await _trashService.DeleteAllItemsAsync().ConfigureAwait(false);
			Items = [];
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error deleting all trash items.");
			TempData["ErrorTitle"] = TrashLoc.DeleteAll;
			TempData["ErrorMessage"] = ex.Message;
			Items = [.. await _trashService.GetItemsAsync().ConfigureAwait(false)];
		}

		if (Request.Headers.ContainsKey("HX-Request"))
		{
			return Partial("_TrashList", this);
		}

		return RedirectToPage();
	}

	public static string GetItemHref(TrashItemModel item)
	{
		return item.Type switch
		{
			ItemType.Note => "/note/" + item.Id,
			ItemType.TodoItem => "/todoitem/" + item.Id,
			_ => "/",
		};
	}
}

