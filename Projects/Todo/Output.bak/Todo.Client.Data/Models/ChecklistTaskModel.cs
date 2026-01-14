using System;
using System.Collections.Generic;
using Todo.Contracts;

namespace Todo.Client.Data.Models;

public class ChecklistTaskModel
{
	public int Id { get; set; }

	public bool IsDone { get; set; }
	public string Description { get; set; } = string.Empty;
}