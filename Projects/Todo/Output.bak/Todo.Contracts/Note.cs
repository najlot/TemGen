using System;
using System.Collections.Generic;

namespace Todo.Contracts;

public class Note
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public PredefinedColor Color { get; set; }
}