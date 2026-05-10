using System;
using Todo.Contracts.Shared;

namespace Todo.Contracts.Favorites;

public sealed class Favorite
{
	public ItemType TargetType { get; set; }
	public Guid ItemId { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
}
