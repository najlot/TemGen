using System;
using Todo.Contracts;

namespace Todo.Contracts.Events;

public class TrashItemDeleted(Guid id, ItemType type)
{
	public Guid Id { get; } = id;
	public ItemType Type { get; } = type;
}
