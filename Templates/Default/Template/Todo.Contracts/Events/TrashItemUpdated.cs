using System;
using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Contracts.Events;

public class TrashItemUpdated
{
	public Guid Id { get; set; }
	public ItemType Type { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public DateTime? DeletedAt { get; set; }
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>