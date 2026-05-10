using System;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.Contracts.Favorites;

public sealed class FavoriteUpdated
{
	public ItemType TargetType { get; set; }
	public Guid ItemId { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>