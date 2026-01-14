using System;

namespace Todo.Contracts.Events;

public class NoteDeleted(Guid id)
{
	public Guid Id { get; } = id;
}