using System;
using Todo.Contracts.Shared;

namespace Todo.Contracts.Favorites;

public sealed class FavoriteDeleted(Guid itemId, ItemType targetType)
{
	public Guid ItemId { get; } = itemId;
	public ItemType TargetType { get; } = targetType;
}
