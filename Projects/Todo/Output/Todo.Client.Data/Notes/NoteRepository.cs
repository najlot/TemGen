using Najlot.Map;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using Todo.Client.Data.Identity;
using Todo.Client.Data;
using Todo.Contracts.Filters;
using Todo.Contracts.Notes;

namespace Todo.Client.Data.Notes;

public class NoteRepository(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider, IMap map)
	: HttpClientRepository(httpClientFactory, tokenProvider), INoteRepository
{
	public async Task<NoteListItemModel[]> GetItemsAsync()
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var items = await client.GetFromJsonAsync<NoteListItem[]>("api/Note", ClientDataJsonSerializer.Options).ConfigureAwait(false) ?? [];
		return map.From<NoteListItem>(items).ToArray<NoteListItemModel>();
	}

	public async Task<NoteListItemModel[]> GetItemsAsync(EntityFilter filter)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var response = await client.PostAsJsonAsync("api/Note/ListFiltered", filter, ClientDataJsonSerializer.Options).ConfigureAwait(false);
		var items = await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<NoteListItem[]>(ClientDataJsonSerializer.Options).ConfigureAwait(false) ?? [];
		return map.From<NoteListItem>(items).ToArray<NoteListItemModel>();
	}

	public async Task<NoteModel> GetItemAsync(Guid id)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var item = await client.GetFromJsonAsync<Note>($"api/Note/{id}", ClientDataJsonSerializer.Options).ConfigureAwait(false);
		return map.From(item).To<NoteModel>();
	}

	public async Task AddItemAsync(NoteModel item)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var request = map.From(item).To<CreateNote>();
		var response = await client.PostAsJsonAsync("api/Note", request, ClientDataJsonSerializer.Options).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task UpdateItemAsync(NoteModel item)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var request = map.From(item).To<UpdateNote>();
		var response = await client.PutAsJsonAsync("api/Note", request, ClientDataJsonSerializer.Options).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task DeleteItemAsync(Guid id)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var response = await client.DeleteAsync($"api/Note/{id}").ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}
}