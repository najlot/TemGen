using System.Threading.Tasks;

namespace Todo.Client.Data.GlobalSearch;

public interface IGlobalSearchRepository
{
	Task<GlobalSearchItemModel[]> SearchAsync(string text);
}
