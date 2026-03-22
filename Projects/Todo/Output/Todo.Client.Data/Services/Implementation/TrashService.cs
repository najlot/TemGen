using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Client.Data.Repositories;
using Todo.Contracts;
using Todo.Contracts.Events;

namespace Todo.Client.Data.Services.Implementation;

public sealed class TrashService(
	IApiEventConnectionProvider apiEventConnectionProvider,
	ITrashRepository repository)
	: ITrashService, IAsyncDisposable
{
	private readonly SemaphoreSlim _listenerLock = new(1, 1);
	private readonly List<IDisposable> _subscriptions = [];
	private bool _eventListenerStarted;

	public event AsyncEventHandler<TrashItemCreated>? ItemCreated;
	public event AsyncEventHandler<TrashItemUpdated>? ItemUpdated;
	public event AsyncEventHandler<TrashItemDeleted>? ItemDeleted;

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

			_subscriptions.Add(connection.On<string>(typeof(TrashItemCreated).Name, async param =>
			{
				if (ItemCreated is { } handler && JsonSerializer.Deserialize<TrashItemCreated>(param) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<TrashItemCreated>>())
					{
						await invocation(this, message).ConfigureAwait(false);
					}
				}
			}));

			_subscriptions.Add(connection.On<string>(typeof(TrashItemUpdated).Name, async param =>
			{
				if (ItemUpdated is { } handler && JsonSerializer.Deserialize<TrashItemUpdated>(param) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<TrashItemUpdated>>())
					{
						await invocation(this, message).ConfigureAwait(false);
					}
				}
			}));

			_subscriptions.Add(connection.On<string>(typeof(TrashItemDeleted).Name, async param =>
			{
				if (ItemDeleted is { } handler && JsonSerializer.Deserialize<TrashItemDeleted>(param) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<TrashItemDeleted>>())
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

	public async Task<TrashItemModel[]> GetItemsAsync()
	{
		return await repository.GetItemsAsync().ConfigureAwait(false);
	}

	public async Task RestoreItemAsync(ItemType type, Guid id)
	{
		await repository.RestoreItemAsync(type, id).ConfigureAwait(false);
	}

	public async Task DeleteItemAsync(ItemType type, Guid id)
	{
		await repository.DeleteItemAsync(type, id).ConfigureAwait(false);
	}

	public async Task DeleteAllItemsAsync()
	{
		await repository.DeleteAllItemsAsync().ConfigureAwait(false);
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
