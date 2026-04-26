using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Contracts.Filters;

namespace <# Project.Namespace#>.Client.Data.Filters;

public static class FilterOperatorOptions
{
	public static readonly FilterOperatorOption ContainsOption = new()
	{
		Value = FilterOperator.Contains,
		Label = FilterOperatorLoc.OperatorContains,
	};

	public static readonly FilterOperatorOption DoesNotContainOption = new()
	{
		Value = FilterOperator.DoesNotContain,
		Label = FilterOperatorLoc.OperatorDoesNotContain,
	};

	public static readonly FilterOperatorOption EqualsOption = new()
	{
		Value = FilterOperator.Equals,
		Label = FilterOperatorLoc.OperatorEquals,
	};

	public static readonly FilterOperatorOption NotEqualsOption = new()
	{
		Value = FilterOperator.NotEquals,
		Label = FilterOperatorLoc.OperatorNotEquals,
	};

	public static readonly FilterOperatorOption StartsWithOption = new()
	{
		Value = FilterOperator.StartsWith,
		Label = FilterOperatorLoc.OperatorStartsWith,
	};

	public static readonly FilterOperatorOption EndsWithOption = new()
	{
		Value = FilterOperator.EndsWith,
		Label = FilterOperatorLoc.OperatorEndsWith,
	};

	public static readonly FilterOperatorOption GreaterThanOption = new()
	{
		Value = FilterOperator.GreaterThan,
		Label = FilterOperatorLoc.OperatorGreaterThan,
	};

	public static readonly FilterOperatorOption GreaterThanOrEqualOption = new()
	{
		Value = FilterOperator.GreaterThanOrEqual,
		Label = FilterOperatorLoc.OperatorGreaterThanOrEqual,
	};

	public static readonly FilterOperatorOption LessThanOption = new()
	{
		Value = FilterOperator.LessThan,
		Label = FilterOperatorLoc.OperatorLessThan,
	};

	public static readonly FilterOperatorOption LessThanOrEqualOption = new()
	{
		Value = FilterOperator.LessThanOrEqual,
		Label = FilterOperatorLoc.OperatorLessThanOrEqual,
	};

	public static readonly FilterOperatorOption IsEmptyOption = new()
	{
		Value = FilterOperator.IsEmpty,
		Label = FilterOperatorLoc.OperatorIsEmpty,
	};

	public static readonly FilterOperatorOption IsNotEmptyOption = new()
	{
		Value = FilterOperator.IsNotEmpty,
		Label = FilterOperatorLoc.OperatorIsNotEmpty,
	};
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>