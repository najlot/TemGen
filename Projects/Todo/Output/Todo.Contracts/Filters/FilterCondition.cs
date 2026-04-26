namespace Todo.Contracts.Filters;

public sealed class FilterCondition
{
	public string Field { get; set; } = string.Empty;
	public FilterOperator Operator { get; set; } = FilterOperator.Equals;
	public string? Value { get; set; }
}
