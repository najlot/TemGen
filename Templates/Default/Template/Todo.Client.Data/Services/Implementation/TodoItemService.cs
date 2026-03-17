using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.Client.Data.Repositories;
using <# Project.Namespace#>.Contracts.Events;
using <# Project.Namespace#>.Contracts.Filters;

namespace <# Project.Namespace#>.Client.Data.Services.Implementation;

public sealed class <# Definition.Name#>Service(
	ITokenProvider tokenProvider,
	IHttpClientFactory httpClientFactory,
	I<# Definition.Name#>Repository repository)
	: I<# Definition.Name#>Service, IAsyncDisposable
{
	private HubConnection? _connection;

	public event AsyncEventHandler<<# Definition.Name#>Created>? ItemCreated;
	public event AsyncEventHandler<<# Definition.Name#>Updated>? ItemUpdated;
	public event AsyncEventHandler<<# Definition.Name#>Deleted>? ItemDeleted;

	public async Task StartEventListener()
	{
		if (_connection is not null)
		{
			return;
		}

		using var client = httpClientFactory.CreateClient();
		var serverUri = (client?.BaseAddress) ?? throw new NullReferenceException("Could not retrieve server connection information!");
		var token = await tokenProvider.GetToken().ConfigureAwait(false);
		var signalRUri = new Uri(serverUri, "/events");

		_connection = new HubConnectionBuilder()
			.WithUrl(
				signalRUri.AbsoluteUri,
				options => options.Headers.Add("Authorization", $"Bearer {token}"))
			.WithAutomaticReconnect()
			.Build();

		_connection.On<string>(typeof(<# Definition.Name#>Created).Name, async param =>
		{
			if (ItemCreated is { } handler && JsonSerializer.Deserialize<<# Definition.Name#>Created>(param) is { } message)
			{
				foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<<# Definition.Name#>Created>>())
				{
					await invocation(this, message).ConfigureAwait(false);
				}
			}
		});

		_connection.On<string>(typeof(<# Definition.Name#>Updated).Name, async param =>
		{
			if (ItemUpdated is { } handler && JsonSerializer.Deserialize<<# Definition.Name#>Updated>(param) is { } message)
			{
				foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<<# Definition.Name#>Updated>>())
				{
					await invocation(this, message).ConfigureAwait(false);
				}
			}
		});

		_connection.On<string>(typeof(<# Definition.Name#>Deleted).Name, async param =>
		{
			if (ItemDeleted is { } handler && JsonSerializer.Deserialize<<# Definition.Name#>Deleted>(param) is { } message)
			{
				foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<<# Definition.Name#>Deleted>>())
				{
					await invocation(this, message).ConfigureAwait(false);
				}
			}
		});

		await _connection.StartAsync().ConfigureAwait(false);
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
#>			<# entry.Field#> = "",
<#elseif entry.IsArray
#>			<# entry.Field#> = [],
<#end#><#end#>
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
		if (_connection is not null)
		{
			await _connection.DisposeAsync();
			_connection = null;
		}
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>