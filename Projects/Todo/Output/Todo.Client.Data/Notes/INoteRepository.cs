using System;
using System.Threading.Tasks;
using Todo.Contracts.Filters;
using Todo.Contracts.Notes;

namespace Todo.Client.Data.Notes;

public interface INoteRepository
{
	Task<NoteListItemModel[]> GetItemsAsync();

	Task<NoteListItemModel[]> GetItemsAsync(EntityFilter filter);

	Task<NoteModel> GetItemAsync(Guid id);

	Task AddItemAsync(NoteModel item);

	Task UpdateItemAsync(NoteModel item);

	Task DeleteItemAsync(Guid id);
}