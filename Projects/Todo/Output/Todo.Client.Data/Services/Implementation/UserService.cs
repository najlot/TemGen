using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Client.Data.Repositories;
using Todo.Contracts.Events;

namespace Todo.Client.Data.Services.Implementation;

public sealed class UserService(
	IApiEventConnectionProvider apiEventConnectionProvider,
	IUserRepository repository)
	: IUserService, IAsyncDisposable
{
	private readonly SemaphoreSlim _listenerLock = new(1, 1);
	private readonly List<IDisposable> _subscriptions = [];
	private bool _eventListenerStarted;

	public event AsyncEventHandler<UserCreated>? ItemCreated;
	public event AsyncEventHandler<UserUpdated>? ItemUpdated;
	public event AsyncEventHandler<UserDeleted>? ItemDeleted;

	public async Task StartEventListener()
	{
		if (_eventListenerStarted)
		{
			return;
		}

		await _listenerLock.WaitAsync().ConfigureAwait(false);

		try
		{
			if (_eventListenerStarted)
			{
				return;
			}

			var connection = await apiEventConnectionProvider.GetConnectionAsync().ConfigureAwait(false);

			_subscriptions.Add(connection.On<string>(typeof(UserCreated).Name, async param =>
			{
				if (ItemCreated is { } handler && JsonSerializer.Deserialize<UserCreated>(param) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<UserCreated>>())
					{
						await invocation(this, message).ConfigureAwait(false);
					}
				}
			}));

			_subscriptions.Add(connection.On<string>(typeof(UserUpdated).Name, async param =>
			{
				if (ItemUpdated is { } handler && JsonSerializer.Deserialize<UserUpdated>(param) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<UserUpdated>>())
					{
						await invocation(this, message).ConfigureAwait(false);
					}
				}
			}));

			_subscriptions.Add(connection.On<string>(typeof(UserDeleted).Name, async param =>
			{
				if (ItemDeleted is { } handler && JsonSerializer.Deserialize<UserDeleted>(param) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<UserDeleted>>())
					{
						await invocation(this, message).ConfigureAwait(false);
					}
				}
			}));

			_eventListenerStarted = true;
		}
		finally
		{
			_listenerLock.Release();
		}
	}

	public UserModel CreateUser()
	{
		return new UserModel()
		{
			Id = Guid.NewGuid(),
			Username = string.Empty,
			EMail = string.Empty,
			Password = string.Empty,
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
		await _listenerLock.WaitAsync().ConfigureAwait(false);

		try
		{
			foreach (var subscription in _subscriptions)
			{
				subscription.Dispose();
			}

			_subscriptions.Clear();
			_eventListenerStarted = false;
		}
		finally
		{
			_listenerLock.Release();
			_listenerLock.Dispose();
		}
	}
}