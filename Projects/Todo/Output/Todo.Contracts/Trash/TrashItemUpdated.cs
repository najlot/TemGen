using System;
using Todo.Contracts.Trash;
using Todo.Contracts.Shared;

namespace Todo.Contracts.Trash;

public class TrashItemUpdated
{
	public Guid Id { get; set; }
	public ItemType Type { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public DateTime? DeletedAt { get; set; }
}
