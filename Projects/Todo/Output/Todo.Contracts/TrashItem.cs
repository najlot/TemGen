using System;

namespace Todo.Contracts;

public enum ItemType
{
	Note,
	TodoItem,
}

public sealed class TrashItem
{
	public Guid Id { get; set; }
	public ItemType Type { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public DateTime? DeletedAt { get; set; }
}
