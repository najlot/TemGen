using System.Threading.Tasks;
using Todo.Client.Data.Models;

namespace Todo.Client.Data.Repositories;

public interface IGlobalSearchRepository
{
	Task<GlobalSearchItemModel[]> SearchAsync(string text);
}
