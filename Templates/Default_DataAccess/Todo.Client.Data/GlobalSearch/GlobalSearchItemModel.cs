using System;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.Client.Data.GlobalSearch;

public sealed class GlobalSearchItemModel
{
	public Guid Id { get; set; }
	public ItemType Type { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>