using System;
using System.Collections.Generic;

namespace Todo.Contracts.Commands;

public class CreateNote(
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