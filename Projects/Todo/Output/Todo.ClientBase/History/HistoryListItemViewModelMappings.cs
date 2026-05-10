using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Contracts.History;

namespace Todo.ClientBase.History;

[Mapping]
internal sealed partial class HistoryListItemViewModelMappings
{
	public static void MapToViewModel(IMap map, HistoryEntry from, HistoryListItemViewModel to)
	{
		to.Username = string.IsNullOrWhiteSpace(from.Username) ? "-" : from.Username;
		to.Changes = from.Changes;
		to.TimeStampText = from.TimeStamp.ToLocalTime().ToString("g");
	}
}
