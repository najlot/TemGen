using System.Collections.Generic;

namespace Todo.Contracts.Filters;

public sealed class EntityFilter
{
	public List<FilterCondition> Conditions { get; set; } = [];
}
