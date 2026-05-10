using System.Threading.Channels;
using Microsoft.AspNetCore.Http;

namespace <# Project.Namespace#>.Htmx.LiveUpdates;

public static class LiveUpdatesEndpoint
{
	private const string TokenClaimType = "access_token";

	public static async Task HandleAsync(
		HttpContext context,
		ILiveUpdateBridgeManager bridgeManager,
		ILoggerFactory loggerFactory)
	{
		var log = loggerFactory.CreateLogger("LiveUpdatesEndpoint");
		var accessToken = context.User?.FindFirst(TokenClaimType)?.Value;

		if (string.IsNullOrWhiteSpace(accessToken))
		{
			log.LogWarning("Rejecting live update stream without an access token.");
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			return;
		}

		ILiveUpdateSubscription subscription;

		try
		{
			subscription = await bridgeManager.SubscribeAsync(accessToken, context.RequestAborted).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			log.LogError(ex, "Could not establish live update stream.");
			context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
			return;
		}

		await using (subscription.ConfigureAwait(false))
		{
			context.Response.ContentType = "text/event-stream";
			context.Response.Headers.CacheControl = "no-cache";
			context.Response.Headers.Append("X-Accel-Buffering", "no");

			await context.Response.WriteAsync("retry: 1000\n\n", context.RequestAborted).ConfigureAwait(false);
			await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);

			using var keepAliveTimer = new PeriodicTimer(TimeSpan.FromSeconds(25));
			var keepAliveTask = keepAliveTimer.WaitForNextTickAsync(context.RequestAborted).AsTask();

			while (!context.RequestAborted.IsCancellationRequested)
			{
				try
				{
					var waitForMessageTask = subscription.Messages.WaitToReadAsync(context.RequestAborted).AsTask();
					var completedTask = await Task.WhenAny(waitForMessageTask, keepAliveTask).ConfigureAwait(false);

					if (completedTask == keepAliveTask)
					{
						if (!await keepAliveTask.ConfigureAwait(false))
						{
							break;
						}

						await context.Response.WriteAsync(": keepalive\n\n", context.RequestAborted).ConfigureAwait(false);
						await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);
						keepAliveTask = keepAliveTimer.WaitForNextTickAsync(context.RequestAborted).AsTask();
						continue;
					}

					if (!await waitForMessageTask.ConfigureAwait(false))
					{
						break;
					}

					while (subscription.Messages.TryRead(out var messageName))
					{
						await WriteEventAsync(context.Response, messageName, context.RequestAborted).ConfigureAwait(false);
					}
				}
				catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
				{
					break;
				}
				catch (ChannelClosedException)
				{
					break;
				}
			}
		}
	}

	private static async Task WriteEventAsync(HttpResponse response, string messageName, CancellationToken cancellationToken)
	{
		await response.WriteAsync($"event: {messageName}\n", cancellationToken).ConfigureAwait(false);
		await response.WriteAsync($"data: {messageName}\n\n", cancellationToken).ConfigureAwait(false);
		await response.Body.FlushAsync(cancellationToken).ConfigureAwait(false);
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>