using System;
using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Contracts.Events;

public class TrashItemDeleted(Guid id, ItemType type)
{
	public Guid Id { get; } = id;
	public ItemType Type { get; } = type;
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>