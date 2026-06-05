using System.Collections.Generic;

namespace <# Project.Namespace#>.Contracts.Filters;

public sealed class EntityFilter
{
	public List<FilterCondition> Conditions { get; set; } = [];
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>