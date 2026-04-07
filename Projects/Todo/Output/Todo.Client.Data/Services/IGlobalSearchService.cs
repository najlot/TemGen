using System.Threading.Tasks;
using Todo.Client.Data.Models;

namespace Todo.Client.Data.Services;

public interface IGlobalSearchService
{
	Task<GlobalSearchItemModel[]> SearchAsync(string text);
}
