using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.Data.Trash;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Contracts.Shared;
using <# Project.Namespace#>.Contracts.Trash;

namespace <# Project.Namespace#>.Htmx.Pages;

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
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.NameLow == "user")).OrderBy(d => d.Name)
#>			ItemType.<# definition.Name#> => "/<#cs Write(definition.Name.ToLower())#>/" + item.Id,
<#end#>			_ => "/",
		};
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
