using System;

namespace <# Project.Namespace#>.Contracts;

public class HistoryEntry
{
	public Guid Id { get; set; }
	public Guid EntityId { get; set; }
	public Guid UserId { get; set; }
	public string Username { get; set; } = string.Empty;
	public DateTime TimeStamp { get; set; }
	public HistoryChange[] Changes { get; set; } = [];
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>