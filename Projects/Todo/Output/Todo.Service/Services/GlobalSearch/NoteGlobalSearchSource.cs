using Najlot.Map;
using Todo.Contracts;
using Todo.Service.Repository;

namespace Todo.Service.Services.GlobalSearch;

public class NoteGlobalSearchSource(
	INoteRepository noteRepository,
	IMap map,
	IPermissionQueryFilter permissionQueryFilter) : IGlobalSearchSource
{
	public IAsyncEnumerable<GlobalSearchItem> SearchAsync(string text)
	{
		var normalizedText = text.ToLower();
		var query = permissionQueryFilter
			.ApplyReadFilter(noteRepository.GetAllQueryable())
			.Where(item => item.DeletedAt == null)
			.Where(item =>
				item.Title.Contains(normalizedText) ||
				item.Content.Contains(normalizedText));

		return map.From(query).To<GlobalSearchItem>().ToAsyncEnumerable();
	}
}