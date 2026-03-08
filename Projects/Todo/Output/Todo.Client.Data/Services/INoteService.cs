using System;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Contracts.Events;
using Todo.Contracts.Filters;

namespace Todo.Client.Data.Services;

public interface INoteService
{
	event AsyncEventHandler<NoteCreated>? OnItemCreated;
	event AsyncEventHandler<NoteUpdated>? OnItemUpdated;
	event AsyncEventHandler<NoteDeleted>? OnItemDeleted;

	Task StartEventListener();

	NoteModel CreateNote();
	Task AddItemAsync(NoteModel item);
	Task<NoteListItemModel[]> GetItemsAsync();
	Task<NoteListItemModel[]> GetItemsAsync(NoteFilter filter);
	Task<NoteModel> GetItemAsync(Guid id);
	Task UpdateItemAsync(NoteModel item);
	Task DeleteItemAsync(Guid id);
}