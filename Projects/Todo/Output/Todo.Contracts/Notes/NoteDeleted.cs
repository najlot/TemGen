using System;

namespace Todo.Contracts.Notes;

public class NoteDeleted(Guid id)
{
	public Guid Id { get; } = id;
}