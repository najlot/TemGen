using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using <# Project.Namespace#>.Contracts.Favorites;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.Client.Data.Favorites;

public sealed class FavoriteService(
	IApiEventConnectionProvider apiEventConnectionProvider,
	IFavoriteRepository repository)
	: IFavoriteService, IAsyncDisposable
{
	private readonly SemaphoreSlim _listenerLock = new(1, 1);
	private readonly List<IDisposable> _subscriptions = [];
	private bool _eventListenerStarted;

	public event AsyncEventHandler<FavoriteCreated>? ItemCreated;
	public event AsyncEventHandler<FavoriteUpdated>? ItemUpdated;
	public event AsyncEventHandler<FavoriteDeleted>? ItemDeleted;

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

			_subscriptions.Add(connection.On<string>(typeof(FavoriteCreated).Name, async param =>
			{
				if (ItemCreated is { } handler && JsonSerializer.Deserialize<FavoriteCreated>(param, ClientDataJsonSerializer.Options) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<FavoriteCreated>>())
					{
						await invocation(this, message).ConfigureAwait(false);
					}
				}
			}));

			_subscriptions.Add(connection.On<string>(typeof(FavoriteUpdated).Name, async param =>
			{
				if (ItemUpdated is { } handler && JsonSerializer.Deserialize<FavoriteUpdated>(param, ClientDataJsonSerializer.Options) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<FavoriteUpdated>>())
					{
						await invocation(this, message).ConfigureAwait(false);
					}
				}
			}));

			_subscriptions.Add(connection.On<string>(typeof(FavoriteDeleted).Name, async param =>
			{
				if (ItemDeleted is { } handler && JsonSerializer.Deserialize<FavoriteDeleted>(param, ClientDataJsonSerializer.Options) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<FavoriteDeleted>>())
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

	public Task<Favorite[]> GetItemsAsync(ItemType targetType)
	{
		return repository.GetItemsAsync(targetType);
	}

	public Task AddItemAsync(ItemType targetType, Guid itemId)
	{
		return repository.AddItemAsync(targetType, itemId);
	}

	public Task DeleteItemAsync(ItemType targetType, Guid itemId)
	{
		return repository.DeleteItemAsync(targetType, itemId);
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
<#cs SetOutputPathAndSkipOtherDefinitions()#>