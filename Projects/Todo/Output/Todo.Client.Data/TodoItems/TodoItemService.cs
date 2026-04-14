using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Todo.Client.Data.Serialization;
using Todo.Contracts.TodoItems;

namespace Todo.Client.Data.TodoItems;

public sealed class TodoItemService(
	IApiEventConnectionProvider apiEventConnectionProvider,
	ITodoItemRepository repository)
	: ITodoItemService, IAsyncDisposable
{
	private readonly SemaphoreSlim _listenerLock = new(1, 1);
	private readonly List<IDisposable> _subscriptions = [];
	private bool _eventListenerStarted;

	public event AsyncEventHandler<TodoItemCreated>? ItemCreated;
	public event AsyncEventHandler<TodoItemUpdated>? ItemUpdated;
	public event AsyncEventHandler<TodoItemDeleted>? ItemDeleted;

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

			_subscriptions.Add(connection.On<string>(typeof(TodoItemCreated).Name, async param =>
			{
				if (ItemCreated is { } handler && JsonSerializer.Deserialize<TodoItemCreated>(param, ClientDataJsonSerializer.Options) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<TodoItemCreated>>())
					{
						await invocation(this, message).ConfigureAwait(false);
					}
				}
			}));

			_subscriptions.Add(connection.On<string>(typeof(TodoItemUpdated).Name, async param =>
			{
				if (ItemUpdated is { } handler && JsonSerializer.Deserialize<TodoItemUpdated>(param, ClientDataJsonSerializer.Options) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<TodoItemUpdated>>())
					{
						await invocation(this, message).ConfigureAwait(false);
					}
				}
			}));

			_subscriptions.Add(connection.On<string>(typeof(TodoItemDeleted).Name, async param =>
			{
				if (ItemDeleted is { } handler && JsonSerializer.Deserialize<TodoItemDeleted>(param, ClientDataJsonSerializer.Options) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<TodoItemDeleted>>())
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

	public TodoItemModel CreateTodoItem()
	{
		return new TodoItemModel()
		{
			Id = Guid.NewGuid(),
			Title = string.Empty,
			Content = string.Empty,
			CreatedBy = string.Empty,
			ChangedBy = string.Empty,
			Priority = string.Empty,
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