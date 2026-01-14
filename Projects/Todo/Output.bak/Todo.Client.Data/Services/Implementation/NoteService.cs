using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Client.Data.Repositories;
using Todo.Contracts.Filters;

namespace Todo.Client.Data.Services.Implementation;

public sealed class NoteService : INoteService
{
	private readonly INoteRepository _repository;

	public NoteService(INoteRepository repository)
	{
		_repository = repository;
	}

	public NoteModel CreateNote()
	{
		return new NoteModel()
		{
			Id = Guid.NewGuid(),
			Title = "",
			Content = ""
		};
	}

	public async Task AddItemAsync(NoteModel item)
	{
		await _repository.AddItemAsync(item);
	}

	public async Task DeleteItemAsync(Guid id)
	{
		await _repository.DeleteItemAsync(id);
	}

	public async Task<NoteModel> GetItemAsync(Guid id)
	{
		return await _repository.GetItemAsync(id);
	}

	public async Task<IEnumerable<NoteListItemModel>> GetItemsAsync()
	{
		return await _repository.GetItemsAsync();
	}

	public async Task<IEnumerable<NoteListItemModel>> GetItemsAsync(NoteFilter filter)
	{
		return await _repository.GetItemsAsync(filter);
	}

	public async Task UpdateItemAsync(NoteModel item)
	{
		await _repository.UpdateItemAsync(item);
	}

	public void Dispose() => _repository.Dispose();
}