using System;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.Contracts.Favorites;

public sealed class CreateFavorite
{
	public ItemType TargetType { get; set; }
	public Guid ItemId { get; set; }
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>