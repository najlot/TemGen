using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Todo.Contracts.Favorites;
using Todo.Contracts.Shared;
using Todo.Service.Infrastructure.Persistence;
using Todo.Service.Shared.Results;

namespace Todo.Service.Features.Favorites;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public sealed class FavoritesController(FavoriteService favoriteService) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<Favorite[]>> List([FromQuery] ItemType targetType)
	{
		var items = await favoriteService.GetItemsAsync(targetType).ToArrayAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpPost]
	public async Task<ActionResult> Create(
		[FromBody] CreateFavorite command,
		[FromServices] IUnitOfWork unitOfWork)
	{
		var result = await favoriteService.CreateFavoriteAsync(command).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}

	[HttpDelete("{targetType}/{itemId}")]
	public async Task<ActionResult> Delete(ItemType targetType, Guid itemId, [FromServices] IUnitOfWork unitOfWork)
	{
		var result = await favoriteService.DeleteItemAsync(targetType, itemId).ConfigureAwait(false);
		if (result.IsFailure)
		{
			return this.ToActionResult(result);
		}

		await unitOfWork.CommitAsync().ConfigureAwait(false);
		return Ok();
	}
}
