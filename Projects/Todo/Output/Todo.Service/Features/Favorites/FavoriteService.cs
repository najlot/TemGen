using Najlot.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Contracts.Favorites;
using Todo.Contracts.Shared;
using Todo.Service.Features.Auth;
using Todo.Service.Shared.Realtime;
using Todo.Service.Shared.Results;

namespace Todo.Service.Features.Favorites;

public sealed class FavoriteService(
	IFavoriteRepository favoriteRepository,
	IEnumerable<IFavoriteSource> sources,
	IUserIdProvider userIdProvider,
	IPublisher publisher,
	IMap map)
{
	private readonly Dictionary<ItemType, IFavoriteSource> _sourcesByType = sources.ToDictionary(source => source.Type);

	public IAsyncEnumerable<Favorite> GetItemsAsync(ItemType targetType)
	{
		var userId = userIdProvider.GetRequiredUserId();
		var query = favoriteRepository
			.GetAllQueryable()
			.Where(item => item.UserId == userId && item.TargetType == targetType);

		return map.From(query).To<Favorite>().ToAsyncEnumerable();
	}

	public async Task<Result> CreateFavoriteAsync(CreateFavorite command)
	{
		if (command.ItemId == Guid.Empty)
		{
			return Result.Validation("Favorite item id is required.");
		}

		var userId = userIdProvider.GetRequiredUserId();
		var alreadyExists = favoriteRepository
			.GetAllQueryable()
			.Any(item => item.UserId == userId
				&& item.TargetType == command.TargetType
				&& item.ItemId == command.ItemId);

		if (alreadyExists)
		{
			return Result.Conflict("Favorite already exists.");
		}

		if (!_sourcesByType.TryGetValue(command.TargetType, out var source))
		{
			return Result.NotFound("Favorite item source not found.");
		}

		var model = await source.CreateFavoriteAsync(userId, command.ItemId).ConfigureAwait(false);
		if (model is null)
		{
			return Result.NotFound("Favorite item not found.");
		}

		model.Id = model.Id == Guid.Empty ? Guid.NewGuid() : model.Id;
		model.UserId = userId;
		model.TargetType = command.TargetType;
		model.ItemId = command.ItemId;

		await favoriteRepository.Insert(model).ConfigureAwait(false);
		await PublishCreatedAsync(model).ConfigureAwait(false);

		return Result.Success();
	}

	public async Task UpdateItemsAsync(ItemType targetType, Guid itemId, string title, string content)
	{
		if (itemId == Guid.Empty)
		{
			return;
		}

		title ??= string.Empty;
		content ??= string.Empty;

		var favorites = favoriteRepository
			.GetAllQueryable()
			.Where(item => item.TargetType == targetType && item.ItemId == itemId)
			.ToArray();

		foreach (var favorite in favorites)
		{
			if (string.Equals(favorite.Title, title, StringComparison.Ordinal)
				&& string.Equals(favorite.Content, content, StringComparison.Ordinal))
			{
				continue;
			}

			favorite.Title = title;
			favorite.Content = content;
			await favoriteRepository.Update(favorite).ConfigureAwait(false);
			await PublishUpdatedAsync(favorite).ConfigureAwait(false);
		}
	}

	public async Task DeleteItemsAsync(ItemType targetType, Guid itemId)
	{
		if (itemId == Guid.Empty)
		{
			return;
		}

		var favorites = favoriteRepository
			.GetAllQueryable()
			.Where(item => item.TargetType == targetType && item.ItemId == itemId)
			.ToArray();

		foreach (var favorite in favorites)
		{
			await favoriteRepository.Delete(favorite.Id).ConfigureAwait(false);
			await PublishDeletedAsync(favorite).ConfigureAwait(false);
		}
	}

	public async Task<Result> DeleteItemAsync(ItemType targetType, Guid itemId)
	{
		if (itemId == Guid.Empty)
		{
			return Result.Validation("Favorite item id is required.");
		}

		var userId = userIdProvider.GetRequiredUserId();
		var favorite = favoriteRepository
			.GetAllQueryable()
			.FirstOrDefault(item => item.UserId == userId
				&& item.TargetType == targetType
				&& item.ItemId == itemId);

		if (favorite is null)
		{
			return Result.NotFound("Favorite not found.");
		}

		await favoriteRepository.Delete(favorite.Id).ConfigureAwait(false);
		await PublishDeletedAsync(favorite).ConfigureAwait(false);
		return Result.Success();
	}

	private Task PublishCreatedAsync(FavoriteModel favorite)
	{
		var message = map.From(favorite).To<FavoriteCreated>();
		return publisher.PublishToUserAsync(favorite.UserId.ToString(), message);
	}

	private Task PublishUpdatedAsync(FavoriteModel favorite)
	{
		var message = map.From(favorite).To<FavoriteUpdated>();
		return publisher.PublishToUserAsync(favorite.UserId.ToString(), message);
	}

	private Task PublishDeletedAsync(FavoriteModel favorite)
	{
		var message = new FavoriteDeleted(favorite.ItemId, favorite.TargetType);
		return publisher.PublishToUserAsync(favorite.UserId.ToString(), message);
	}
}
