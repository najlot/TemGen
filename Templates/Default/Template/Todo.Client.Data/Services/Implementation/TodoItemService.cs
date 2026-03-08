using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.Data.Identity;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Client.Data.Repositories;
using <#cs Write(Project.Namespace)#>.Contracts.Events;
using <#cs Write(Project.Namespace)#>.Contracts.Filters;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Services.Implementation;

public sealed class <#cs Write(Definition.Name)#>Service(
	ITokenProvider tokenProvider,
	IHttpClientFactory httpClientFactory,
	I<#cs Write(Definition.Name)#>Repository repository)
	: I<#cs Write(Definition.Name)#>Service, IAsyncDisposable
{
	private HubConnection? _connection;

	public event AsyncEventHandler<<#cs Write(Definition.Name)#>Created>? ItemCreated;
	public event AsyncEventHandler<<#cs Write(Definition.Name)#>Updated>? ItemUpdated;
	public event AsyncEventHandler<<#cs Write(Definition.Name)#>Deleted>? ItemDeleted;

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

		_connection.On<string>(typeof(<#cs Write(Definition.Name)#>Created).Name, async param =>
		{
			if (ItemCreated is { } handler && JsonSerializer.Deserialize<<#cs Write(Definition.Name)#>Created>(param) is { } message)
			{
				foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<<#cs Write(Definition.Name)#>Created>>())
				{
					await invocation(this, message).ConfigureAwait(false);
				}
			}
		});

		_connection.On<string>(typeof(<#cs Write(Definition.Name)#>Updated).Name, async param =>
		{
			if (ItemUpdated is { } handler && JsonSerializer.Deserialize<<#cs Write(Definition.Name)#>Updated>(param) is { } message)
			{
				foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<<#cs Write(Definition.Name)#>Updated>>())
				{
					await invocation(this, message).ConfigureAwait(false);
				}
			}
		});

		_connection.On<string>(typeof(<#cs Write(Definition.Name)#>Deleted).Name, async param =>
		{
			if (ItemDeleted is { } handler && JsonSerializer.Deserialize<<#cs Write(Definition.Name)#>Deleted>(param) is { } message)
			{
				foreach (var invocation in handler.GetInvocationList().Cast<AsyncEventHandler<<#cs Write(Definition.Name)#>Deleted>>())
				{
					await invocation(this, message).ConfigureAwait(false);
				}
			}
		});

		await _connection.StartAsync().ConfigureAwait(false);
	}

	public <#cs Write(Definition.Name)#>Model Create<#cs Write(Definition.Name)#>()
	{
		return new <#cs Write(Definition.Name)#>Model()
		{
			Id = Guid.NewGuid(),
<#cs
foreach(var entry in Entries)
{
	if (entry.IsOwnedType)
	{
		WriteLine($"			{entry.Field} = new (),");
	}
	else if(entry.EntryType.ToLower() == "string")
	{
		WriteLine($"			{entry.Field} = \"\",");
	}
	else if(entry.IsArray)
	{
		WriteLine($"			{entry.Field} = [],");
	}
	
}

Result = Result.TrimEnd(' ', '\r', '\n', ',');
#>
		};
	}

	public async Task AddItemAsync(<#cs Write(Definition.Name)#>Model item)
	{
		await repository.AddItemAsync(item);
	}

	public async Task DeleteItemAsync(Guid id)
	{
		await repository.DeleteItemAsync(id);
	}

	public async Task<<#cs Write(Definition.Name)#>Model> GetItemAsync(Guid id)
	{
		return await repository.GetItemAsync(id);
	}

	public async Task<<#cs Write(Definition.Name)#>ListItemModel[]> GetItemsAsync()
	{
		return await repository.GetItemsAsync();
	}

	public async Task<<#cs Write(Definition.Name)#>ListItemModel[]> GetItemsAsync(<#cs Write(Definition.Name)#>Filter filter)
	{
		return await repository.GetItemsAsync(filter);
	}

	public async Task UpdateItemAsync(<#cs Write(Definition.Name)#>Model item)
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