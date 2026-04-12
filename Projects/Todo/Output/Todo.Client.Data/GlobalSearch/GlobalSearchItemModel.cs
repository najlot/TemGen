using System;
using Todo.Contracts.Shared;

namespace Todo.Client.Data.GlobalSearch;

public sealed class GlobalSearchItemModel
{
	public Guid Id { get; set; }
	public ItemType Type { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
}
