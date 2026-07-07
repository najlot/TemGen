using System;
using <# Project.Namespace#>.Contracts.Trash;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.Contracts.Trash;

public class TrashItemDeleted
{
	public Guid Id { get; set; }
	public ItemType Type { get; set; }
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>