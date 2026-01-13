using System;
using System.Collections.Generic;

namespace Todo.Contracts.Filters;

public sealed class NoteFilter
{
	public string? Title { get; set; }
	public string? Content { get; set; }
	public PredefinedColor? Color { get; set; }
}