using System;
using <# Project.Namespace#>.Contracts.Trash;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.Contracts.Trash;

public class TrashItemDeleted(Guid id, ItemType type)
{
	public Guid Id { get; } = id;
	public ItemType Type { get; } = type;
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>