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

public class TodoItemRepository : ITodoItemRepository
{
	private readonly IRequestClient _client;
	private readonly ITokenProvider _tokenProvider;
	private readonly IMap _map;

	public TodoItemRepository(IRequestClient client, ITokenProvider tokenProvider, IMap map)
	{
		_tokenProvider = tokenProvider;
		_client = client;
		_map = map;
	}

	public async Task<TodoItemListItemModel[]> GetItemsAsync()
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var items = await _client.GetAsync<TodoItemListItem[]>("api/TodoItem", headers);
		return _map.From<TodoItemListItem>(items).ToArray<TodoItemListItemModel>();
	}

	public async Task<TodoItemListItemModel[]> GetItemsAsync(TodoItemFilter filter)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var items = await _client.PostAsync<List<TodoItemListItem>, TodoItemFilter>("api/TodoItem/ListFiltered", filter, headers);
		return _map.From<TodoItemListItem>(items).ToArray<TodoItemListItemModel>();
	}

	public async Task<TodoItemModel> GetItemAsync(Guid id)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var item = await _client.GetAsync<TodoItem>($"api/TodoItem/{id}", headers);
		return _map.From(item).To<TodoItemModel>();
	}

	public async Task AddItemAsync(TodoItemModel item)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var request = _map.From(item).To<CreateTodoItem>();
		await _client.PostAsync("api/TodoItem", request, headers);
	}

	public async Task UpdateItemAsync(TodoItemModel item)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var request = _map.From(item).To<UpdateTodoItem>();
		await _client.PutAsync("api/TodoItem", request, headers);
	}

	public async Task DeleteItemAsync(Guid id)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var response = await _client.DeleteAsync($"api/TodoItem/{id}", headers);
		response.EnsureSuccessStatusCode();
	}

	public void Dispose() => _client.Dispose();
}