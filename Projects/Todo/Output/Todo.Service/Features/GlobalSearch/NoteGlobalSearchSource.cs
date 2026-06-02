using Najlot.Map;
using Todo.Contracts.GlobalSearch;
using Todo.Service.Features.Auth;
using Todo.Service.Features.Notes;

namespace Todo.Service.Features.GlobalSearch;

public class NoteGlobalSearchSource(
	INoteRepository noteRepository,
	IMap map,
	IPermissionService permissionService) : IGlobalSearchSource
{
	public IAsyncEnumerable<GlobalSearchItem> SearchAsync(string text)
	{
		var normalizedText = text.ToLower();
		var query = permissionService
			.ApplyReadFilter(noteRepository.GetAllQueryable())
			.Where(item => item.DeletedAt == null)
			.Where(item =>
				item.Title.ToLower().Contains(normalizedText) ||
				item.Content.ToLower().Contains(normalizedText));

		return map.From(query).To<GlobalSearchItem>().ToAsyncEnumerable();
	}
}