using Todo.Contracts;

namespace Todo.Service.Services.GlobalSearch;

public interface IGlobalSearchSource
{
	IAsyncEnumerable<GlobalSearchItem> SearchAsync(string text);
}
