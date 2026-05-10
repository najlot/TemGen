using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Todo.Client.Data;
using Todo.Client.Data.Favorites;
using Todo.Client.Data.Notes;using Todo.Client.Data.Identity;
using Todo.Client.Localisation;
using Todo.Contracts.Shared;
using Todo.Contracts.Notes;

namespace Todo.Htmx.Pages;

[Authorize]
public class NoteModel : PageModel
{
	private readonly INoteService _noteService;
	private readonly IFavoriteService _favoriteService;
	private readonly ILogger<NoteModel> _log;

	[BindProperty]
	public Todo.Client.Data.Notes.NoteModel? Model { get; set; }


	public bool IsNew { get; private set; }
	public bool IsFavorite { get; private set; }
	public Guid FavoriteItemId { get; private set; }
	public string FavoriteButtonClass => IsFavorite ? "btn btn-outline-warning px-2" : "btn btn-outline-secondary px-2";
	public string FavoriteIcon => IsFavorite ? "star" : "star_outline";
	public string FavoriteTitle => IsFavorite ? "Remove favorite" : "Add favorite";
	public string? ErrorMessage { get; set; }

	public NoteModel(
		INoteService noteService,
		IFavoriteService favoriteService,
		ILogger<NoteModel> log)
	{
		_noteService = noteService;
		_favoriteService = favoriteService;
		_log = log;
	}

	private async Task LoadReferencesAsync()
	{
		await Task.CompletedTask;
	}

	private async Task LoadFavoriteStateAsync(Guid id)
	{
		FavoriteItemId = id;

		if (id == Guid.Empty)
		{
			IsFavorite = false;
			return;
		}

		var favorites = await _favoriteService.GetItemsAsync(ItemType.Note);
		IsFavorite = favorites.Any(item => item.ItemId == id);
	}

	public async Task<IActionResult> OnGetAsync(Guid id)
	{
		try
		{
			await LoadReferencesAsync();

			if (id == Guid.Empty)
			{
				Model = _noteService.CreateNote();
				IsNew = true;
			}
			else
			{
				Model = await _noteService.GetItemAsync(id);
				await LoadFavoriteStateAsync(id);
			}
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error loading Note.");
			TempData["ErrorTitle"] = ErrorLoc.Error;
			TempData["ErrorMessage"] = ex.Message;
		}

		return Page();
	}

	public async Task<IActionResult> OnPostAsync(Guid id, [FromForm] string? action)
	{
		if (id != Guid.Empty)
		{
			await LoadFavoriteStateAsync(id);
		}

		// Handle array child operations
		// Save
		if (!ModelState.IsValid)
		{
			await LoadReferencesAsync();
			IsNew = id == Guid.Empty;
			await LoadFavoriteStateAsync(id);

			TempData["ErrorTitle"] = ErrorLoc.Error;
			TempData["ErrorMessage"] = string.Join("\r\n", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

			return Page();
		}

		if (Model is null)
		{
			TempData["ErrorTitle"] = ErrorLoc.Error;
			TempData["ErrorMessage"] = ErrorLoc.ErrorModelIsNullReload;
			return Page();
		}

		try
		{
			if (id == Guid.Empty)
			{
				Model.Id = id = Guid.NewGuid();
				await _noteService.AddItemAsync(Model);
				return RedirectToPage("/Note", new { id = Model.Id });
			}
			else
			{
				Model.Id = id;
				await _noteService.UpdateItemAsync(Model);
			}
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error saving Note.");
			ErrorMessage = ErrorLoc.ErrorCouldNotSave;
			await LoadReferencesAsync();
			IsNew = id == Guid.Empty;
			await LoadFavoriteStateAsync(id);
			return Page();
		}

		return RedirectToPage("/Note", new { id = id });
	}

	public async Task<IActionResult> OnPostToggleFavoriteAsync(Guid id)
	{
		try
		{
			var favorites = await _favoriteService.GetItemsAsync(ItemType.Note);
			var isFavorite = favorites.Any(item => item.ItemId == id);

			if (isFavorite)
			{
				await _favoriteService.DeleteItemAsync(ItemType.Note, id);
				IsFavorite = false;
			}
			else
			{
				await _favoriteService.AddItemAsync(ItemType.Note, id);
				IsFavorite = true;
			}

			FavoriteItemId = id;
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error toggling favorite for Note.");
			TempData["ErrorTitle"] = ErrorLoc.Error;
			TempData["ErrorMessage"] = ErrorLoc.ErrorCouldNotSave;
			return RedirectToPage("/Note", new { id = id });
		}

		if (Request.Headers.ContainsKey("HX-Request"))
		{
			return Partial("_NoteFavoriteToggle", this);
		}

		return RedirectToPage("/Note", new { id = id });
	}

	public async Task<IActionResult> OnPostDeleteAsync(Guid id)
	{
		try
		{
			await _noteService.DeleteItemAsync(id);
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error deleting Note.");
			TempData["ErrorTitle"] = ErrorLoc.Error;
			TempData["ErrorMessage"] = ErrorLoc.ErrorDeleting;
			return RedirectToPage("/Note", new { id = id });
		}

		if (Request.Headers.ContainsKey("HX-Request"))
		{
			Response.Headers["HX-Redirect"] = "/Notes";
			return Content(string.Empty);
		}

		return RedirectToPage("/Note");
	}
}

