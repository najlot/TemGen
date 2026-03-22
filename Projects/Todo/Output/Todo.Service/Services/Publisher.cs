using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Todo.Service.Services;

public class MessageHub : Hub
{
}

public interface IPublisher
{
	Task PublishAsync<T>(T message) where T : notnull;

	Task PublishToUserAsync<T>(string userId, T message) where T : notnull;
}

public interface IOutboxPublisher
{
	Task FlushAsync();

	void Clear();
}

internal sealed record OutboxMessage(string MessageName, string Content, string? UserId = null);

public class Publisher(IHubContext<MessageHub> hub) : IPublisher, IOutboxPublisher
{
	private readonly List<OutboxMessage> _messages = [];

	public Task PublishAsync<T>(T message) where T : notnull
	{
		var json = JsonSerializer.Serialize(message);
		_messages.Add(new OutboxMessage(typeof(T).Name, json));
		return Task.CompletedTask;
	}

	public Task PublishToUserAsync<T>(string userId, T message) where T : notnull
	{
		var json = JsonSerializer.Serialize(message);
		_messages.Add(new OutboxMessage(typeof(T).Name, json, userId));
		return Task.CompletedTask;
	}

	public async Task FlushAsync()
	{
		foreach (var message in _messages)
		{
			if (string.IsNullOrWhiteSpace(message.UserId))
			{
				await hub.Clients.All.SendAsync(message.MessageName, message.Content).ConfigureAwait(false);
			}
			else
			{
				await hub.Clients.User(message.UserId).SendAsync(message.MessageName, message.Content).ConfigureAwait(false);
			}
		}

		_messages.Clear();
	}

	public void Clear() => _messages.Clear();
}

public class OutboxPublisherMiddleware(RequestDelegate next, ILogger<OutboxPublisherMiddleware> logger)
{
	public async Task InvokeAsync(HttpContext context, IOutboxPublisher outboxPublisher)
	{
		var publishQueuedMessages = false;

		context.Response.OnCompleted(async () =>
		{
			if (!publishQueuedMessages)
			{
				outboxPublisher.Clear();
				return;
			}

			try
			{
				await outboxPublisher.FlushAsync().ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed to flush queued outbox messages.");
				outboxPublisher.Clear();
			}
		});

		try
		{
			await next(context).ConfigureAwait(false);
			publishQueuedMessages = !context.RequestAborted.IsCancellationRequested
				&& context.Response.StatusCode is >= StatusCodes.Status200OK and < StatusCodes.Status400BadRequest;
		}
		catch
		{
			outboxPublisher.Clear();
			throw;
		}
	}
}
