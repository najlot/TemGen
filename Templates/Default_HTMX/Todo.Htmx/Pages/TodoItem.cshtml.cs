using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using <# Project.Namespace#>.Client.Data;
using <# Project.Namespace#>.Client.Data.Favorites;
using <# Project.Namespace#>.Client.Data.<# Definition.Name#>s;<#if Entries.Any(e => e.IsReference)
#>
<#for entry in Entries.Where(e => e.IsReference).DistinctBy(e => e.ReferenceType)
#>using <# Project.Namespace#>.Client.Data.<# entry.ReferenceType#>s;
<#end#><#end#>using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Contracts.Shared;
using <# Project.Namespace#>.Contracts.<# Definition.Name#>s;

namespace <# Project.Namespace#>.Htmx.Pages;

[Authorize]
public class <# Definition.Name#>Model : PageModel
{
	private readonly I<# Definition.Name#>Service _<# Definition.NameLow#>Service;
	private readonly IFavoriteService _favoriteService;
<#for entry in Entries.Where(e => e.IsReference).DistinctBy(e => e.ReferenceType)
#>	private readonly I<# entry.ReferenceType#>Service _<# entry.ReferenceTypeLow#>Service;
<#end#>	private readonly ILogger<<# Definition.Name#>Model> _log;

	[BindProperty]
	public <# Project.Namespace#>.Client.Data.<# Definition.Name#>s.<# Definition.Name#>Model? Model { get; set; }

<#for entry in Entries.Where(e => e.IsReference).DistinctBy(e => e.ReferenceType)
#>	public IReadOnlyList<<# entry.ReferenceType#>ListItemModel> <# entry.ReferenceTypeLow#>s { get; private set; } = [];
<#end#>
	public bool IsNew { get; private set; }
	public bool IsFavorite { get; private set; }
	public Guid FavoriteItemId { get; private set; }
	public string FavoriteButtonClass => IsFavorite ? "btn btn-outline-warning px-2" : "btn btn-outline-secondary px-2";
	public string FavoriteIcon => IsFavorite ? "star" : "star_outline";
	public string FavoriteTitle => IsFavorite ? "Remove favorite" : "Add favorite";
	public string? ErrorMessage { get; set; }

	public <# Definition.Name#>Model(
		I<# Definition.Name#>Service <# Definition.NameLow#>Service,
		IFavoriteService favoriteService,
<#for entry in Entries.Where(e => e.IsReference).DistinctBy(e => e.ReferenceType)
#>		I<# entry.ReferenceType#>Service <# entry.ReferenceTypeLow#>Service,
<#end#>		ILogger<<# Definition.Name#>Model> log)
	{
		_<# Definition.NameLow#>Service = <# Definition.NameLow#>Service;
		_favoriteService = favoriteService;
<#for entry in Entries.Where(e => e.IsReference).DistinctBy(e => e.ReferenceType)
#>		_<# entry.ReferenceTypeLow#>Service = <# entry.ReferenceTypeLow#>Service;
<#end#>		_log = log;
	}

	private async Task LoadReferencesAsync()
	{
<#if Entries.Any(e => e.IsReference)
#><#for entry in Entries.Where(e => e.IsReference).DistinctBy(e => e.ReferenceType)
#>		var available<# entry.ReferenceType#>s = await _<# entry.ReferenceTypeLow#>Service.GetItemsAsync();
		<# entry.ReferenceTypeLow#>s = [new <# entry.ReferenceType#>ListItemModel { Id = Guid.Empty }, .. available<# entry.ReferenceType#>s];
<#end#><#else#>		await Task.CompletedTask;
<#end#>	}

	private async Task LoadFavoriteStateAsync(Guid id)
	{
		FavoriteItemId = id;

		if (id == Guid.Empty)
		{
			IsFavorite = false;
			return;
		}

		var favorites = await _favoriteService.GetItemsAsync(ItemType.<# Definition.Name#>);
		IsFavorite = favorites.Any(item => item.ItemId == id);
	}

	public async Task<IActionResult> OnGetAsync(Guid id)
	{
		try
		{
			await LoadReferencesAsync();

			if (id == Guid.Empty)
			{
				Model = _<# Definition.NameLow#>Service.Create<# Definition.Name#>();
				IsNew = true;
			}
			else
			{
				Model = await _<# Definition.NameLow#>Service.GetItemAsync(id);
				await LoadFavoriteStateAsync(id);
			}
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error loading <# Definition.Name#>.");
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
<#for entry in Entries.Where(e => e.IsArray)
#>		if (action == "add_<#cs Write(entry.Field.ToLower())#>")
		{
			var max = Model?.<# entry.Field#>.Count > 0 ? Model.<# entry.Field#>.Max(e => e.Id) + 1 : 0;
			Model?.<# entry.Field#>.Add(new <# entry.EntryType#>Model { Id = max });
			await LoadReferencesAsync();
			IsNew = id == Guid.Empty;
			return Page();
		}

		if (action?.StartsWith("remove_<#cs Write(entry.Field.ToLower())#>_") == true &&
			int.TryParse(action["remove_<#cs Write(entry.Field.ToLower())#>_".Length..], out var remove<# entry.Field#>Id))
		{
			var toRemove = Model?.<# entry.Field#>.FirstOrDefault(e => e.Id == remove<# entry.Field#>Id);
			if (toRemove != null) Model?.<# entry.Field#>.Remove(toRemove);
			await LoadReferencesAsync();
			IsNew = id == Guid.Empty;
			return Page();
		}

<#end#>		// Save
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
				await _<# Definition.NameLow#>Service.AddItemAsync(Model);
				return RedirectToPage("/<#cs
var name = Definition.Name;
Write(name);
#>", new { id = Model.Id });
			}
			else
			{
				Model.Id = id;
				await _<# Definition.NameLow#>Service.UpdateItemAsync(Model);
			}
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error saving <# Definition.Name#>.");
			ErrorMessage = ErrorLoc.ErrorCouldNotSave;
			await LoadReferencesAsync();
			IsNew = id == Guid.Empty;
			await LoadFavoriteStateAsync(id);
			return Page();
		}

		return RedirectToPage("/<#cs Write(Definition.Name)#>", new { id = id });
	}

	public async Task<IActionResult> OnPostToggleFavoriteAsync(Guid id)
	{
		try
		{
			var favorites = await _favoriteService.GetItemsAsync(ItemType.<# Definition.Name#>);
			var isFavorite = favorites.Any(item => item.ItemId == id);

			if (isFavorite)
			{
				await _favoriteService.DeleteItemAsync(ItemType.<# Definition.Name#>, id);
				IsFavorite = false;
			}
			else
			{
				await _favoriteService.AddItemAsync(ItemType.<# Definition.Name#>, id);
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
			_log.LogError(ex, "Error toggling favorite for <# Definition.Name#>.");
			TempData["ErrorTitle"] = ErrorLoc.Error;
			TempData["ErrorMessage"] = ErrorLoc.ErrorCouldNotSave;
			return RedirectToPage("/<#cs Write(Definition.Name)#>", new { id = id });
		}

		if (Request.Headers.ContainsKey("HX-Request"))
		{
			return Partial("_<# Definition.Name#>FavoriteToggle", this);
		}

		return RedirectToPage("/<#cs Write(Definition.Name)#>", new { id = id });
	}

	public async Task<IActionResult> OnPostDeleteAsync(Guid id)
	{
		try
		{
			await _<# Definition.NameLow#>Service.DeleteItemAsync(id);
		}
		catch (SessionUnavailableException)
		{
			return RedirectToPage("/Identity/Login");
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error deleting <# Definition.Name#>.");
			TempData["ErrorTitle"] = ErrorLoc.Error;
			TempData["ErrorMessage"] = ErrorLoc.ErrorDeleting;
			return RedirectToPage("/<#cs Write(Definition.Name)#>", new { id = id });
		}

		if (Request.Headers.ContainsKey("HX-Request"))
		{
			Response.Headers["HX-Redirect"] = "/<# Definition.Name#>s";
			return Content(string.Empty);
		}

		return RedirectToPage("/<# Definition.Name#>");
	}
}
<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>
