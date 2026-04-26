using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Todo.Client.Data;
using Todo.Contracts.Filters;
using Todo.Contracts.Notes;

namespace Todo.Client.Data.Notes;

public sealed class NoteService(
	IApiEventConnectionProvider apiEventConnectionProvider,
	INoteRepository repository)
	: INoteService, IAsyncDisposable
{
	private readonly SemaphoreSlim _listenerLock = new(1, 1);
	private readonly List<IDisposable> _subscriptions = [];
	private bool _eventListenerStarted;

	public event AsyncEventHandler<NoteCreated>? ItemCreated;
	public event AsyncEventHandler<NoteUpdated>? ItemUpdated;
	public event AsyncEventHandler<NoteDeleted>? ItemDeleted;

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

			_subscriptions.Add(connection.On<string>(typeof(NoteCreated).Name, async param =>
			{
				if (ItemCreated is { } handler && JsonSerializer.Deserialize<NoteCreated>(param, ClientDataJsonSerializer.Options) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<NoteCreated>>())
					{
						await invocation(this, message).ConfigureAwait(false);
					}
				}
			}));

			_subscriptions.Add(connection.On<string>(typeof(NoteUpdated).Name, async param =>
			{
				if (ItemUpdated is { } handler && JsonSerializer.Deserialize<NoteUpdated>(param, ClientDataJsonSerializer.Options) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<NoteUpdated>>())
					{
						await invocation(this, message).ConfigureAwait(false);
					}
				}
			}));

			_subscriptions.Add(connection.On<string>(typeof(NoteDeleted).Name, async param =>
			{
				if (ItemDeleted is { } handler && JsonSerializer.Deserialize<NoteDeleted>(param, ClientDataJsonSerializer.Options) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<NoteDeleted>>())
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

	public NoteModel CreateNote()
	{
		return new NoteModel()
		{
			Id = Guid.NewGuid(),
			Title = string.Empty,
			Content = string.Empty
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

	public async Task<NoteListItemModel[]> GetItemsAsync(EntityFilter filter)
	{
		return await repository.GetItemsAsync(filter);
	}

	public async Task UpdateItemAsync(NoteModel item)
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