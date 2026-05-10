using System.Threading.Tasks;
using Todo.Contracts.History;

namespace Todo.Client.Data.History;

public interface IEntityHistoryLocalizer
{
	bool CanLocalize(string entityName);
	Task<HistoryChange> Localize(HistoryChange change);
}
