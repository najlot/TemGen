using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Todo.Client.Data.Identity;
using Todo.Client.Data.Models;
using Todo.Client.Data.Repositories;
using Todo.Contracts.Events;
using Todo.Contracts.Filters;

namespace Todo.Client.Data.Services.Implementation;

public sealed class TodoItemService(
	ITokenProvider tokenProvider,
	IHttpClientFactory httpClientFactory,
	ITodoItemRepository repository)
	: ITodoItemService, IAsyncDisposable
{
	private HubConnection? _connection;

	public event AsyncEventHandler<TodoItemCreated>? OnItemCreated;
	public event AsyncEventHandler<TodoItemUpdated>? OnItemUpdated;
	public event AsyncEventHandler<TodoItemDeleted>? OnItemDeleted;

	public async Task StartEventListener()
	{
		if (_connection is not null)
		{
			return;
		}

		using var client = httpClientFactory.CreateClient();
		var serverUri = (client?.BaseAddress) ?? throw new NullReferenceException("Could not retrieve server connection information!");
		var token = await tokenProvider.GetToken().ConfigureAwait(false);
		var signalRUri = new Uri(serverUri, "/cosei");

		_connection = new HubConnectionBuilder()
			.WithUrl(
				signalRUri.AbsoluteUri,
				options => options.Headers.Add("Authorization", $"Bearer {token}"))
			.WithAutomaticReconnect()
			.Build();

		_connection.On<string>(typeof(TodoItemCreated).Name, async param =>
		{
			if (OnItemCreated is { } handler && JsonSerializer.Deserialize<TodoItemCreated>(param) is { } message)
			{
				foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<TodoItemCreated>>())
				{
					await invocation(this, message).ConfigureAwait(false);
				}
			}
		});

		_connection.On<string>(typeof(TodoItemUpdated).Name, async param =>
		{
			if (OnItemUpdated is { } handler && JsonSerializer.Deserialize<TodoItemUpdated>(param) is { } message)
			{
				foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<TodoItemUpdated>>())
				{
					await invocation(this, message).ConfigureAwait(false);
				}
			}
		});

		_connection.On<string>(typeof(TodoItemDeleted).Name, async param =>
		{
			if (OnItemDeleted is { } handler && JsonSerializer.Deserialize<TodoItemDeleted>(param) is { } message)
			{
				foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<TodoItemDeleted>>())
				{
					await invocation(this, message).ConfigureAwait(false);
				}
			}
		});

		await _connection.StartAsync().ConfigureAwait(false);
	}

	public TodoItemModel CreateTodoItem()
	{
		return new TodoItemModel()
		{
			Id = Guid.NewGuid(),
			Title = "",
			Content = "",
			CreatedBy = "",
			ChangedBy = "",
			Priority = "",
			Checklist = []
		};
	}

	public async Task AddItemAsync(TodoItemModel item)
	{
		await repository.AddItemAsync(item);
	}

	public async Task DeleteItemAsync(Guid id)
	{
		await repository.DeleteItemAsync(id);
	}

	public async Task<TodoItemModel> GetItemAsync(Guid id)
	{
		return await repository.GetItemAsync(id);
	}

	public async Task<TodoItemListItemModel[]> GetItemsAsync()
	{
		return await repository.GetItemsAsync();
	}

	public async Task<TodoItemListItemModel[]> GetItemsAsync(TodoItemFilter filter)
	{
		return await repository.GetItemsAsync(filter);
	}

	public async Task UpdateItemAsync(TodoItemModel item)
	{
		await repository.UpdateItemAsync(item);
	}

	public async ValueTask DisposeAsync()
	{
		if (_connection is not null)
		{
			await _connection.DisposeAsync();
			_connection = null;
		}
	}
}