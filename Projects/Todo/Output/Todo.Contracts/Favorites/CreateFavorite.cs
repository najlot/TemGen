using System;
using Todo.Contracts.Shared;

namespace Todo.Contracts.Favorites;

public sealed class CreateFavorite
{
	public ItemType TargetType { get; set; }
	public Guid ItemId { get; set; }
}
