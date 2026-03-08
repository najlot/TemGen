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

namespace Todo.Client.Data.Services.Implementation;

public sealed class UserService(
	ITokenProvider tokenProvider,
	IHttpClientFactory httpClientFactory,
	IUserRepository repository)
	: IUserService, IAsyncDisposable
{
	private HubConnection? _connection;

	public event AsyncEventHandler<UserCreated>? OnItemCreated;
	public event AsyncEventHandler<UserUpdated>? OnItemUpdated;
	public event AsyncEventHandler<UserDeleted>? OnItemDeleted;

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

		_connection.On<string>(typeof(UserCreated).Name, async param =>
		{
			if (OnItemCreated is { } handler && JsonSerializer.Deserialize<UserCreated>(param) is { } message)
			{
				foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<UserCreated>>())
				{
					await invocation(this, message).ConfigureAwait(false);
				}
			}
		});

		_connection.On<string>(typeof(UserUpdated).Name, async param =>
		{
			if (OnItemUpdated is { } handler && JsonSerializer.Deserialize<UserUpdated>(param) is { } message)
			{
				foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<UserUpdated>>())
				{
					await invocation(this, message).ConfigureAwait(false);
				}
			}
		});

		_connection.On<string>(typeof(UserDeleted).Name, async param =>
		{
			if (OnItemDeleted is { } handler && JsonSerializer.Deserialize<UserDeleted>(param) is { } message)
			{
				foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<UserDeleted>>())
				{
					await invocation(this, message).ConfigureAwait(false);
				}
			}
		});

		await _connection.StartAsync().ConfigureAwait(false);
	}

	public UserModel CreateUser()
	{
		return new UserModel()
		{
			Id = Guid.NewGuid(),
			Username = "",
			EMail = "",
			Password = "",
		};
	}

	public async Task AddItemAsync(UserModel item)
	{
		await repository.AddItemAsync(item);
	}

	public async Task DeleteItemAsync(Guid id)
	{
		await repository.DeleteItemAsync(id);
	}

	public async Task<UserModel> GetCurrentUserAsync()
	{
		return await repository.GetCurrentUserAsync();
	}

	public async Task<UserModel> GetItemAsync(Guid id)
	{
		return await repository.GetItemAsync(id);
	}

	public async Task<UserListItemModel[]> GetItemsAsync()
	{
		return await repository.GetItemsAsync();
	}

	public async Task UpdateItemAsync(UserModel item)
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