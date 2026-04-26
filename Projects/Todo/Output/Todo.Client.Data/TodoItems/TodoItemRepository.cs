using Najlot.Map;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using Todo.Client.Data.Identity;
using Todo.Client.Data;
using Todo.Contracts.Filters;
using Todo.Contracts.TodoItems;

namespace Todo.Client.Data.TodoItems;

public class TodoItemRepository(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider, IMap map)
	: HttpClientRepository(httpClientFactory, tokenProvider), ITodoItemRepository
{
	public async Task<TodoItemListItemModel[]> GetItemsAsync()
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var items = await client.GetFromJsonAsync<TodoItemListItem[]>("api/TodoItem", ClientDataJsonSerializer.Options).ConfigureAwait(false) ?? [];
		return map.From<TodoItemListItem>(items).ToArray<TodoItemListItemModel>();
	}

	public async Task<TodoItemListItemModel[]> GetItemsAsync(EntityFilter filter)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var response = await client.PostAsJsonAsync("api/TodoItem/ListFiltered", filter, ClientDataJsonSerializer.Options).ConfigureAwait(false);
		var items = await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<TodoItemListItem[]>(ClientDataJsonSerializer.Options).ConfigureAwait(false) ?? [];
		return map.From<TodoItemListItem>(items).ToArray<TodoItemListItemModel>();
	}

	public async Task<TodoItemModel> GetItemAsync(Guid id)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var item = await client.GetFromJsonAsync<TodoItem>($"api/TodoItem/{id}", ClientDataJsonSerializer.Options).ConfigureAwait(false);
		return map.From(item).To<TodoItemModel>();
	}

	public async Task AddItemAsync(TodoItemModel item)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var request = map.From(item).To<CreateTodoItem>();
		var response = await client.PostAsJsonAsync("api/TodoItem", request, ClientDataJsonSerializer.Options).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task UpdateItemAsync(TodoItemModel item)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var request = map.From(item).To<UpdateTodoItem>();
		var response = await client.PutAsJsonAsync("api/TodoItem", request, ClientDataJsonSerializer.Options).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task DeleteItemAsync(Guid id)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var response = await client.DeleteAsync($"api/TodoItem/{id}").ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}
}