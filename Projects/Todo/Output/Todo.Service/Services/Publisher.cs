using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Threading.Tasks;

namespace Todo.Service.Services;

public interface IPublisher
{
	Task PublishAsync<T>(T message) where T : notnull;

	Task PublishToUserAsync<T>(string userId, T message) where T : notnull;
}

public class Publisher(IHubContext<MessageHub> hub) : IPublisher
{
	public async Task PublishAsync<T>(T message) where T : notnull
	{
		var content = JsonSerializer.Serialize(message);
		await hub.Clients.All.SendAsync(typeof(T).Name, content).ConfigureAwait(false);
	}

	public async Task PublishToUserAsync<T>(string userId, T message) where T : notnull
	{
		var content = JsonSerializer.Serialize(message);
		await hub.Clients.User(userId).SendAsync(typeof(T).Name, content).ConfigureAwait(false);
	}
}
