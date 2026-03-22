using System;
using Todo.Contracts;

namespace Todo.Client.Data.Models;

public sealed class GlobalSearchItemModel
{
	public Guid Id { get; set; }
	public ItemType Type { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
}
