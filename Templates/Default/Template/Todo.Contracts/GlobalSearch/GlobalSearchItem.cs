using System;
using <# Project.Namespace#>.Contracts.Trash;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.Contracts.GlobalSearch;

public sealed class GlobalSearchItem
{
	public Guid Id { get; set; }
	public ItemType Type { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>