using Najlot.Map.Attributes;
using System.Linq.Expressions;
using Todo.Contracts;
using Todo.Service.Features.Notes;

namespace Todo.Service.Features.GlobalSearch;

[Mapping]
internal partial class NoteGlobalSearchMappings
{
	public static Expression<Func<NoteModel, GlobalSearchItem>> GetNoteExpression()
	{
		return from => new GlobalSearchItem
		{
			Id = from.Id,
			Type = ItemType.Note,
			Title = from.Title,
			Content = from.Content,
		};
	}
}
