using System;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Contracts.Filters;

namespace Todo.Client.Data.Repositories;

public interface INoteRepository : IDisposable
{
	Task<NoteListItemModel[]> GetItemsAsync();

	Task<NoteListItemModel[]> GetItemsAsync(NoteFilter filter);

	Task<NoteModel> GetItemAsync(Guid id);

	Task AddItemAsync(NoteModel item);

	Task UpdateItemAsync(NoteModel item);

	Task DeleteItemAsync(Guid id);
}