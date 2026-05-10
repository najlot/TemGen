using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Todo.Client.Data;
using Todo.Client.Data.Favorites;
using Todo.Client.Data.TodoItems;
using Todo.Client.Data.Users;
using Todo.Client.Data.Identity;
using Todo.Client.Localisation;
using Todo.Contracts.Shared;
using Todo.Contracts.TodoItems;

namespace Todo.Htmx.Pages;

[Authorize]
public class TodoItemModel : PageModel
{
	private readonly ITodoItemService _todoItemService;
	private readonly IFavoriteService _favoriteService;
	private readonly IUserService _userService;
	private readonly ILogger<TodoItemModel> _log;

	[BindProperty]
	public Todo.Client.Data.TodoItems.TodoItemModel? Model { get; set; }

	public IReadOnlyList<UserListItemModel> users { get; private set; } = [];

	public bool IsNew { get; private set; }
	public bool IsFavorite { get; private set; }
	public Guid FavoriteItemId { get; private set; }
	public string FavoriteButtonClass => IsFavorite ? "btn btn-outline-warning px-2" : "btn btn-outline-secondary px-2";
	public string FavoriteIcon => IsFavorite ? "star" : "star_outline";
	public string FavoriteTitle => IsFavorite ? "Remove favorite" : "Add favorite";
	public string? ErrorMessage { get; set; }

	public TodoItemModel(
		ITodoItemService todoItemService,
		IFavoriteService favoriteService,
		IUserService userService,
		ILogger<TodoItemModel> log)
	{
		_todoItemService = todoItemService;
		_favoriteService = favoriteService;
		_userService = userService;
		_log = log;
	}

	private async Task LoadReferencesAsync()
	{
		var availableUsers = await _userService.GetItemsAsync();
		users = [new UserListItemModel { Id = Guid.Empty }, .. availableUsers];
	}

	private async Task LoadFavoriteStateAsync(Guid id)
	{
		FavoriteItemId = id;

		if (id == Guid.Empty)
		{
			IsFavorite = false;
			return;
		}

		var favorites = await _favoriteService.GetItemsAsync(ItemType.TodoItem);
		IsFavorite = favorites.Any(item => item.ItemId == id);
	}

	public async Task<IActionResult> OnGetAsync(Guid id)
	{
		try
		{
			await LoadReferencesAsync();

			if (id == Guid.Empty)
			{
				Model = _todoItemService.CreateTodoItem();
				IsNew = true;
			}
			else
			{
				Model = await _todoItemService.GetItemAsync(id);
				await LoadFavoriteStateAsync(id);
			}
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error loading TodoItem.");
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
		if (action == "add_checklist")
		{
			var max = Model?.Checklist.Count > 0 ? Model.Checklist.Max(e => e.Id) + 1 : 0;
			Model?.Checklist.Add(new ChecklistTaskModel { Id = max });
			await LoadReferencesAsync();
			IsNew = id == Guid.Empty;
			return Page();
		}

		if (action?.StartsWith("remove_checklist_") == true &&
			int.TryParse(action["remove_checklist_".Length..], out var removeChecklistId))
		{
			var toRemove = Model?.Checklist.FirstOrDefault(e => e.Id == removeChecklistId);
			if (toRemove != null) Model?.Checklist.Remove(toRemove);
			await LoadReferencesAsync();
			IsNew = id == Guid.Empty;
			return Page();
		}

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
				await _todoItemService.AddItemAsync(Model);
				return RedirectToPage("/TodoItem", new { id = Model.Id });
			}
			else
			{
				Model.Id = id;
				await _todoItemService.UpdateItemAsync(Model);
			}
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error saving TodoItem.");
			ErrorMessage = ErrorLoc.ErrorCouldNotSave;
			await LoadReferencesAsync();
			IsNew = id == Guid.Empty;
			await LoadFavoriteStateAsync(id);
			return Page();
		}

		return RedirectToPage("/TodoItem", new { id = id });
	}

	public async Task<IActionResult> OnPostToggleFavoriteAsync(Guid id)
	{
		try
		{
			var favorites = await _favoriteService.GetItemsAsync(ItemType.TodoItem);
			var isFavorite = favorites.Any(item => item.ItemId == id);

			if (isFavorite)
			{
				await _favoriteService.DeleteItemAsync(ItemType.TodoItem, id);
				IsFavorite = false;
			}
			else
			{
				await _favoriteService.AddItemAsync(ItemType.TodoItem, id);
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
			_log.LogError(ex, "Error toggling favorite for TodoItem.");
			TempData["ErrorTitle"] = ErrorLoc.Error;
			TempData["ErrorMessage"] = ErrorLoc.ErrorCouldNotSave;
			return RedirectToPage("/TodoItem", new { id = id });
		}

		if (Request.Headers.ContainsKey("HX-Request"))
		{
			return Partial("_TodoItemFavoriteToggle", this);
		}

		return RedirectToPage("/TodoItem", new { id = id });
	}

	public async Task<IActionResult> OnPostDeleteAsync(Guid id)
	{
		try
		{
			await _todoItemService.DeleteItemAsync(id);
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error deleting TodoItem.");
			TempData["ErrorTitle"] = ErrorLoc.Error;
			TempData["ErrorMessage"] = ErrorLoc.ErrorDeleting;
			return RedirectToPage("/TodoItem", new { id = id });
		}

		if (Request.Headers.ContainsKey("HX-Request"))
		{
			Response.Headers["HX-Redirect"] = "/TodoItems";
			return Content(string.Empty);
		}

		return RedirectToPage("/TodoItem");
	}
}

