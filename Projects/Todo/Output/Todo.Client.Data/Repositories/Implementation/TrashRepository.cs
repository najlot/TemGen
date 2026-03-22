using Najlot.Map;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Todo.Client.Data.Identity;
using Todo.Client.Data.Models;
using Todo.Contracts;

namespace Todo.Client.Data.Repositories.Implementation;

public sealed class TrashRepository(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider, IMap map)
	: HttpClientRepository(httpClientFactory, tokenProvider), ITrashRepository
{
	public async Task<TrashItemModel[]> GetItemsAsync()
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var items = await client.GetFromJsonAsync<TrashItem[]>("api/Trash").ConfigureAwait(false) ?? [];
		return map.From<TrashItem>(items).ToArray<TrashItemModel>();
	}

	public async Task RestoreItemAsync(ItemType type, Guid id)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var response = await client.PostAsync($"api/Trash/Restore/{type}/{id}", null).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task DeleteItemAsync(ItemType type, Guid id)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var response = await client.DeleteAsync($"api/Trash/{type}/{id}").ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task DeleteAllItemsAsync()
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var response = await client.DeleteAsync("api/Trash/All").ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}
}
