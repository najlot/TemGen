using Todo.Contracts.GlobalSearch;

namespace Todo.Service.Features.GlobalSearch;

public interface IGlobalSearchSource
{
	IAsyncEnumerable<GlobalSearchItem> SearchAsync(string text);
}
