using System;
using System.Collections.Generic;
using Todo.Contracts;

namespace Todo.Client.Data.Models;

public class NoteModel
{
	public Guid Id { get; set; }

	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public PredefinedColor Color { get; set; }
}