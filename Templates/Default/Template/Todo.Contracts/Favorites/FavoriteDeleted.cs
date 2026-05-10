using System;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.Contracts.Favorites;

public sealed class FavoriteDeleted(Guid itemId, ItemType targetType)
{
	public Guid ItemId { get; } = itemId;
	public ItemType TargetType { get; } = targetType;
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>