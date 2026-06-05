namespace <# Project.Namespace#>.Contracts.Filters;

public static class FilterOperatorExtensions
{
	public static bool RequiresValue(this FilterOperator @operator)
		=> @operator is not FilterOperator.IsEmpty and not FilterOperator.IsNotEmpty;
}

public enum FilterOperator
{
	Equals,
	NotEquals,
	Contains,
	DoesNotContain,
	StartsWith,
	EndsWith,
	GreaterThan,
	GreaterThanOrEqual,
	LessThan,
	LessThanOrEqual,
	IsEmpty,
	IsNotEmpty,
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>