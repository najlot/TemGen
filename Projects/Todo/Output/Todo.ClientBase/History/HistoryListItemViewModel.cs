using Todo.Contracts.History;

namespace Todo.ClientBase.History;

public sealed class HistoryListItemViewModel
{
	public string Username { get; set; } = "-";
	public string TimeStampText { get; set; } = string.Empty;
	public HistoryChange[] Changes { get; set; } = [];
	public int ChangeCount => Changes.Length;
}
