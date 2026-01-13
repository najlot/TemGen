using Cosei.Client.Base;
using Najlot.Map;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Client.Data.Identity;
using Todo.Contracts;
using Todo.Contracts.Commands;
using Todo.Contracts.Filters;
using Todo.Contracts.ListItems;

namespace Todo.Client.Data.Repositories.Implementation;

public class NoteRepository : INoteRepository
{
	private readonly IRequestClient _client;
	private readonly ITokenProvider _tokenProvider;
	private readonly IMap _map;

	public NoteRepository(IRequestClient client, ITokenProvider tokenProvider, IMap map)
	{
		_tokenProvider = tokenProvider;
		_client = client;
		_map = map;
	}

	public async Task<NoteListItemModel[]> GetItemsAsync()
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var items = await _client.GetAsync<NoteListItem[]>("api/Note", headers);
		return _map.From<NoteListItem>(items).ToArray<NoteListItemModel>();
	}

	public async Task<NoteListItemModel[]> GetItemsAsync(NoteFilter filter)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var items = await _client.PostAsync<List<NoteListItem>, NoteFilter>("api/Note/ListFiltered", filter, headers);
		return _map.From<NoteListItem>(items).ToArray<NoteListItemModel>();
	}

	public async Task<NoteModel> GetItemAsync(Guid id)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var item = await _client.GetAsync<Note>($"api/Note/{id}", headers);
		return _map.From(item).To<NoteModel>();
	}

	public async Task AddItemAsync(NoteModel item)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var request = _map.From(item).To<CreateNote>();
		await _client.PostAsync("api/Note", request, headers);
	}

	public async Task UpdateItemAsync(NoteModel item)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var request = _map.From(item).To<UpdateNote>();
		await _client.PutAsync("api/Note", request, headers);
	}

	public async Task DeleteItemAsync(Guid id)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var response = await _client.DeleteAsync($"api/Note/{id}", headers);
		response.EnsureSuccessStatusCode();
	}

	public void Dispose() => _client.Dispose();
}