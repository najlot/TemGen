using System;
using System.Collections.Generic;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.Contracts.Filters;

public sealed class Filter
{
	public Guid Id { get; set; }
	public ItemType TargetType { get; set; }
	public string Name { get; set; } = string.Empty;
	public bool IsDefault { get; set; }
	public List<FilterCondition> Conditions { get; set; } = [];
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>