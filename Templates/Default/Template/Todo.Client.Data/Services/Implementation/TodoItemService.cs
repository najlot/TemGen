using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.Client.Data.Repositories;
using <# Project.Namespace#>.Contracts.Events;
using <# Project.Namespace#>.Contracts.Filters;

namespace <# Project.Namespace#>.Client.Data.Services.Implementation;

public sealed class <# Definition.Name#>Service(
	IApiEventConnectionProvider apiEventConnectionProvider,
	I<# Definition.Name#>Repository repository)
	: I<# Definition.Name#>Service, IAsyncDisposable
{
	private readonly SemaphoreSlim _listenerLock = new(1, 1);
	private readonly List<IDisposable> _subscriptions = [];
	private bool _eventListenerStarted;

	public event AsyncEventHandler<<# Definition.Name#>Created>? ItemCreated;
	public event AsyncEventHandler<<# Definition.Name#>Updated>? ItemUpdated;
	public event AsyncEventHandler<<# Definition.Name#>Deleted>? ItemDeleted;

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

			_subscriptions.Add(connection.On<string>(typeof(<# Definition.Name#>Created).Name, async param =>
			{
				if (ItemCreated is { } handler && JsonSerializer.Deserialize<<# Definition.Name#>Created>(param) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<<# Definition.Name#>Created>>())
					{
						await invocation(this, message).ConfigureAwait(false);
					}
				}
			}));

			_subscriptions.Add(connection.On<string>(typeof(<# Definition.Name#>Updated).Name, async param =>
			{
				if (ItemUpdated is { } handler && JsonSerializer.Deserialize<<# Definition.Name#>Updated>(param) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<<# Definition.Name#>Updated>>())
					{
						await invocation(this, message).ConfigureAwait(false);
					}
				}
			}));

			_subscriptions.Add(connection.On<string>(typeof(<# Definition.Name#>Deleted).Name, async param =>
			{
				if (ItemDeleted is { } handler && JsonSerializer.Deserialize<<# Definition.Name#>Deleted>(param) is { } message)
				{
					foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<<# Definition.Name#>Deleted>>())
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

	public <# Definition.Name#>Model Create<# Definition.Name#>()
	{
		return new <# Definition.Name#>Model()
		{
			Id = Guid.NewGuid(),
<#for entry in Entries
#><#if entry.IsOwnedType
#>			<# entry.Field#> = new (),
<#elseif entry.EntryType.ToLower() == "string"
#>			<# entry.Field#> = string.Empty,
<#elseif entry.IsArray
#>			<# entry.Field#> = [],
<#end#><#end#><#cs Result = Result.TrimEnd(',', '\n','\r')#>
		};
	}

	public async Task AddItemAsync(<# Definition.Name#>Model item)
	{
		await repository.AddItemAsync(item);
	}

	public async Task DeleteItemAsync(Guid id)
	{
		await repository.DeleteItemAsync(id);
	}

	public async Task<<# Definition.Name#>Model> GetItemAsync(Guid id)
	{
		return await repository.GetItemAsync(id);
	}

	public async Task<<# Definition.Name#>ListItemModel[]> GetItemsAsync()
	{
		return await repository.GetItemsAsync();
	}

	public async Task<<# Definition.Name#>ListItemModel[]> GetItemsAsync(<# Definition.Name#>Filter filter)
	{
		return await repository.GetItemsAsync(filter);
	}

	public async Task UpdateItemAsync(<# Definition.Name#>Model item)
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
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>