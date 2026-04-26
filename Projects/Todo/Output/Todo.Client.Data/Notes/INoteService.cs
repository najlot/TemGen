using System;
using System.Threading.Tasks;
using Todo.Contracts.Filters;
using Todo.Contracts.Notes;

namespace Todo.Client.Data.Notes;

public interface INoteService
{
	event AsyncEventHandler<NoteCreated>? ItemCreated;
	event AsyncEventHandler<NoteUpdated>? ItemUpdated;
	event AsyncEventHandler<NoteDeleted>? ItemDeleted;

	Task StartEventListener();

	NoteModel CreateNote();
	Task AddItemAsync(NoteModel item);
	Task<NoteListItemModel[]> GetItemsAsync();
	Task<NoteListItemModel[]> GetItemsAsync(EntityFilter filter);
	Task<NoteModel> GetItemAsync(Guid id);
	Task UpdateItemAsync(NoteModel item);
	Task DeleteItemAsync(Guid id);
}