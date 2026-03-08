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

public sealed class NoteService(
	ITokenProvider tokenProvider,
	IHttpClientFactory httpClientFactory,
	INoteRepository repository)
	: INoteService, IAsyncDisposable
{
	private HubConnection? _connection;

	public event AsyncEventHandler<NoteCreated>? ItemCreated;
	public event AsyncEventHandler<NoteUpdated>? ItemUpdated;
	public event AsyncEventHandler<NoteDeleted>? ItemDeleted;

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

		_connection.On<string>(typeof(NoteCreated).Name, async param =>
		{
			if (ItemCreated is { } handler && JsonSerializer.Deserialize<NoteCreated>(param) is { } message)
			{
				foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<NoteCreated>>())
				{
					await invocation(this, message).ConfigureAwait(false);
				}
			}
		});

		_connection.On<string>(typeof(NoteUpdated).Name, async param =>
		{
			if (ItemUpdated is { } handler && JsonSerializer.Deserialize<NoteUpdated>(param) is { } message)
			{
				foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<NoteUpdated>>())
				{
					await invocation(this, message).ConfigureAwait(false);
				}
			}
		});

		_connection.On<string>(typeof(NoteDeleted).Name, async param =>
		{
			if (ItemDeleted is { } handler && JsonSerializer.Deserialize<NoteDeleted>(param) is { } message)
			{
				foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<NoteDeleted>>())
				{
					await invocation(this, message).ConfigureAwait(false);
				}
			}
		});

		await _connection.StartAsync().ConfigureAwait(false);
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
		await repository.AddItemAsync(item);
	}

	public async Task DeleteItemAsync(Guid id)
	{
		await repository.DeleteItemAsync(id);
	}

	public async Task<NoteModel> GetItemAsync(Guid id)
	{
		return await repository.GetItemAsync(id);
	}

	public async Task<NoteListItemModel[]> GetItemsAsync()
	{
		return await repository.GetItemsAsync();
	}

	public async Task<NoteListItemModel[]> GetItemsAsync(NoteFilter filter)
	{
		return await repository.GetItemsAsync(filter);
	}

	public async Task UpdateItemAsync(NoteModel item)
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