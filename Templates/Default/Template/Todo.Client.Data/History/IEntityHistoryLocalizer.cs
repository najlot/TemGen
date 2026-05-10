using System.Threading.Tasks;
using <# Project.Namespace#>.Contracts.History;

namespace <# Project.Namespace#>.Client.Data.History;

public interface IEntityHistoryLocalizer
{
	bool CanLocalize(string entityName);
	Task<HistoryChange> Localize(HistoryChange change);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>