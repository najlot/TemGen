using System;
using System.Collections.Generic;

namespace Todo.Contracts.Events;

public class NoteUpdated(
	Guid id,
	string title,
	string content,
	PredefinedColor color)
{
	public Guid Id { get; } = id;
	public string Title { get; } = title;
	public string Content { get; } = content;
	public PredefinedColor Color { get; } = color;
}