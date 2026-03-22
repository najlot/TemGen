using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Todo.Client.Data.Identity;

namespace Todo.Client.Data.Services.Implementation;

public sealed class ApiEventConnectionProvider(
	ITokenProvider tokenProvider,
	IHttpClientFactory httpClientFactory)
	: IApiEventConnectionProvider, IAsyncDisposable
{
	private readonly SemaphoreSlim _connectionLock = new(1, 1);
	private HubConnection? _connection;

	public async Task<HubConnection> GetConnectionAsync()
	{
		if (_connection is { State: not HubConnectionState.Disconnected })
		{
			return _connection;
		}

		await _connectionLock.WaitAsync().ConfigureAwait(false);

		try
		{
			if (_connection is null)
			{
				using var client = httpClientFactory.CreateClient();
				var serverUri = client.BaseAddress ?? throw new NullReferenceException("Could not retrieve server connection information!");
				var signalRUri = new Uri(serverUri, "/events");

				_connection = new HubConnectionBuilder()
					.WithUrl(
						signalRUri.AbsoluteUri,
						options => options.AccessTokenProvider = async () => await tokenProvider.GetToken().ConfigureAwait(false))
					.WithAutomaticReconnect()
					.Build();
			}

			if (_connection.State == HubConnectionState.Disconnected)
			{
				await _connection.StartAsync().ConfigureAwait(false);
			}

			return _connection;
		}
		finally
		{
			_connectionLock.Release();
		}
	}

	public async ValueTask DisposeAsync()
	{
		await _connectionLock.WaitAsync().ConfigureAwait(false);

		try
		{
			if (_connection is not null)
			{
				await _connection.DisposeAsync().ConfigureAwait(false);
				_connection = null;
			}
		}
		finally
		{
			_connectionLock.Release();
			_connectionLock.Dispose();
		}
	}
}