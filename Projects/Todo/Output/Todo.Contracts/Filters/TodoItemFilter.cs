using System;
using System.Collections.Generic;

namespace Todo.Contracts.Filters;

public sealed class TodoItemFilter
{
	public string? Title { get; set; }
	public string? Content { get; set; }
	public DateTime? CreatedAtFrom { get; set; }
	public DateTime? CreatedAtTo { get; set; }
	public string? CreatedBy { get; set; }
		public Guid? AssignedToId { get; set; }
	public TodoItemStatus? Status { get; set; }
	public DateTime? ChangedAtFrom { get; set; }
	public DateTime? ChangedAtTo { get; set; }
	public string? ChangedBy { get; set; }
	public string? Priority { get; set; }
}