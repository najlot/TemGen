using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR.Client;

namespace <# Project.Namespace#>.Htmx.LiveUpdates;

public interface ILiveUpdateBridgeManager
{
	Task<ILiveUpdateSubscription> SubscribeAsync(string accessToken, CancellationToken cancellationToken);
}

public interface ILiveUpdateSubscription : IAsyncDisposable
{
	ChannelReader<string> Messages { get; }
}

public sealed class LiveUpdateBridgeManager(
	IHttpClientFactory httpClientFactory,
	ILogger<LiveUpdateBridgeManager> log)
	: ILiveUpdateBridgeManager
{
	public async Task<ILiveUpdateSubscription> SubscribeAsync(string accessToken, CancellationToken cancellationToken)
	{
		using var client = httpClientFactory.CreateClient();
		var serverUri = client.BaseAddress ?? throw new InvalidOperationException("Could not retrieve server connection information!");
		var signalRUri = new Uri(serverUri, "/events");

		var connection = new HubConnectionBuilder()
			.WithUrl(signalRUri.AbsoluteUri, options => options.AccessTokenProvider = () => Task.FromResult<string?>(accessToken))
			.WithAutomaticReconnect()
			.Build();

		var bridge = new BrowserEventBridge(connection, log);
		foreach (var messageName in LiveUpdateMessageCatalog.MessageNames)
		{
			bridge.Register(connection.On<string>(messageName, _ =>
			{
				bridge.Enqueue(messageName);
				return Task.CompletedTask;
			}));
		}

		connection.Reconnected += _ =>
		{
			bridge.Enqueue(LiveUpdateMessageCatalog.FullRefreshMessageName);
			return Task.CompletedTask;
		};

		connection.Closed += error =>
		{
			if (error is not null)
			{
				log.LogDebug(error, "Live update bridge closed.");
			}

			bridge.Complete(error);
			return Task.CompletedTask;
		};

		try
		{
			await connection.StartAsync(cancellationToken).ConfigureAwait(false);
			return bridge;
		}
		catch
		{
			await bridge.DisposeAsync().ConfigureAwait(false);
			throw;
		}
	}

	private sealed class BrowserEventBridge(HubConnection connection, ILogger<LiveUpdateBridgeManager> log) : ILiveUpdateSubscription
	{
		private readonly Channel<string> _messages = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
		{
			SingleReader = true,
			SingleWriter = false
		});
		private readonly List<IDisposable> _subscriptions = [];
		private int _disposed;

		public ChannelReader<string> Messages => _messages.Reader;

		public void Register(IDisposable subscription)
		{
			_subscriptions.Add(subscription);
		}

		public void Enqueue(string messageName)
		{
			if (!_messages.Writer.TryWrite(messageName))
			{
				log.LogDebug("Dropped live update message {MessageName} because the SSE stream is closing.", messageName);
			}
		}

		public void Complete(Exception? error = null)
		{
			_messages.Writer.TryComplete(error);
		}

		public async ValueTask DisposeAsync()
		{
			if (Interlocked.Exchange(ref _disposed, 1) != 0)
			{
				return;
			}

			foreach (var subscription in _subscriptions)
			{
				subscription.Dispose();
			}

			_subscriptions.Clear();
			Complete();
			await connection.DisposeAsync().ConfigureAwait(false);
		}
	}
}

internal static class LiveUpdateMessageCatalog
{
	public const string FullRefreshMessageName = "LiveRefreshAll";

	public static IReadOnlyList<string> MessageNames { get; } =
	[
		"TrashItemCreated",
		"TrashItemUpdated",
		"TrashItemDeleted",
		"FavoriteCreated",
		"FavoriteUpdated",
		"FavoriteDeleted",
<#for definition in Definitions.Where(d => !(d.IsArray || d.IsEnumeration || d.IsOwnedType))
#>		"<# definition.Name#>Created",
		"<# definition.Name#>Updated",
		"<# definition.Name#>Deleted",
<#end#>	];
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>