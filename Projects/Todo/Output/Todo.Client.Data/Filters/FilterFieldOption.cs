using System.Collections.Generic;
using Todo.Contracts.Filters;

namespace Todo.Client.Data.Filters;

public sealed class FilterValueOption
{
	public string Value { get; init; } = string.Empty;
	public string Label { get; init; } = string.Empty;
}

public sealed class FilterOperatorOption
{
	public FilterOperator Value { get; init; }
	public string Label { get; init; } = string.Empty;
}

public sealed class FilterFieldOption
{
	public string Key { get; init; } = string.Empty;
	public string Label { get; init; } = string.Empty;
	public FilterFieldKind Kind { get; init; }
	public IReadOnlyList<FilterOperatorOption> Operators { get; init; } = [];
	public IReadOnlyList<FilterValueOption> Values { get; init; } = [];
}
