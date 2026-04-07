namespace <# Project.Namespace#>.Contracts;

public class HistoryChange
{
	public string Path { get; set; } = string.Empty;
	public string OldValue { get; set; } = string.Empty;
	public string NewValue { get; set; } = string.Empty;
}<#cs SetOutputPathAndSkipOtherDefinitions()#>