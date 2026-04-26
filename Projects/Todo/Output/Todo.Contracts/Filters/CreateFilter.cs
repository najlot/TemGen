using System;
using System.Collections.Generic;
using Todo.Contracts.Shared;

namespace Todo.Contracts.Filters;

public sealed class CreateFilter
{
	public Guid Id { get; set; }
	public ItemType TargetType { get; set; }
	public string Name { get; set; } = string.Empty;
	public bool IsDefault { get; set; }
	public List<FilterCondition> Conditions { get; set; } = [];
}
