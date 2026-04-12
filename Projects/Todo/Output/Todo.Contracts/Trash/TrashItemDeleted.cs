using System;
using Todo.Contracts.Trash;
using Todo.Contracts.Shared;

namespace Todo.Contracts.Trash;

public class TrashItemDeleted(Guid id, ItemType type)
{
	public Guid Id { get; } = id;
	public ItemType Type { get; } = type;
}
