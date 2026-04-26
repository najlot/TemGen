using System.Collections.Generic;

namespace <# Project.Namespace#>.Client.Data.Filters;

public static class FilterOperatorCatalog
{
	private static readonly IReadOnlyList<FilterOperatorOption> _textOptions =
	[
		FilterOperatorOptions.ContainsOption,
		FilterOperatorOptions.DoesNotContainOption,
		FilterOperatorOptions.EqualsOption,
		FilterOperatorOptions.NotEqualsOption,
		FilterOperatorOptions.StartsWithOption,
		FilterOperatorOptions.EndsWithOption
	];

	private static readonly IReadOnlyList<FilterOperatorOption> _textOptionsWithEmpty =
	[
		.. _textOptions,
		FilterOperatorOptions.IsEmptyOption,
		FilterOperatorOptions.IsNotEmptyOption
	];

	public static IReadOnlyList<FilterOperatorOption> CreateTextOptions(bool allowEmpty = true)
		=> allowEmpty ? _textOptionsWithEmpty : _textOptions;

	private static readonly IReadOnlyList<FilterOperatorOption> _equalityOptions =
	[
		FilterOperatorOptions.EqualsOption,
		FilterOperatorOptions.NotEqualsOption
	];

	private static readonly IReadOnlyList<FilterOperatorOption> _equalityOptionsWithEmpty =
	[
		.. _equalityOptions,
		FilterOperatorOptions.IsEmptyOption,
		FilterOperatorOptions.IsNotEmptyOption
	];

	public static IReadOnlyList<FilterOperatorOption> CreateEqualityOptions(bool allowEmpty = false)
		=> allowEmpty ? _equalityOptionsWithEmpty : _equalityOptions;

	private static readonly IReadOnlyList<FilterOperatorOption> _comparableOptions =
	[
		FilterOperatorOptions.EqualsOption,
		FilterOperatorOptions.NotEqualsOption,
		FilterOperatorOptions.GreaterThanOption,
		FilterOperatorOptions.GreaterThanOrEqualOption,
		FilterOperatorOptions.LessThanOption,
		FilterOperatorOptions.LessThanOrEqualOption
	];

	private static readonly IReadOnlyList<FilterOperatorOption> _comparableOptionsWithEmpty =
	[
		.. _comparableOptions,
		FilterOperatorOptions.IsEmptyOption,
		FilterOperatorOptions.IsNotEmptyOption
	];

	public static IReadOnlyList<FilterOperatorOption> CreateComparableOptions(bool allowEmpty = false)
		=> allowEmpty ? _comparableOptionsWithEmpty : _comparableOptions;
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>