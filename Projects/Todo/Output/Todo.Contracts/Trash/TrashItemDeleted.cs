using System;
using Todo.Contracts.Trash;
using Todo.Contracts.Shared;

namespace Todo.Contracts.Trash;

public class TrashItemDeleted
{
	public Guid Id { get; set; }
	public ItemType Type { get; set; }
}
