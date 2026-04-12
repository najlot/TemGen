using System.Threading.Tasks;

namespace Todo.Client.Data.GlobalSearch;

public interface IGlobalSearchService
{
	Task<GlobalSearchItemModel[]> SearchAsync(string text);
}
