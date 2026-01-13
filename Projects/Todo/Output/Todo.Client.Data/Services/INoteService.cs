using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Contracts.Filters;

namespace Todo.Client.Data.Services;

public interface INoteService : IDisposable
{
	NoteModel CreateNote();
	Task AddItemAsync(NoteModel item);
	Task<IEnumerable<NoteListItemModel>> GetItemsAsync();
	Task<IEnumerable<NoteListItemModel>> GetItemsAsync(NoteFilter filter);
	Task<NoteModel> GetItemAsync(Guid id);
	Task UpdateItemAsync(NoteModel item);
	Task DeleteItemAsync(Guid id);
}