using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Todo.Service.Hubs;

namespace Todo.Service.Publisher;

public class SignalRPublisher(IHubContext<NotificationHub> hubContext) : IPublisher
{
	public Task PublishAsync<T>(T message) where T : class
	{
		var typeName = typeof(T).Name;
		return hubContext.Clients.All.SendAsync(typeName, message);
	}
}
