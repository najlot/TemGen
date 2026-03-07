using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Service.Hubs;

namespace <#cs Write(Project.Namespace)#>.Service.Publisher;

public class SignalRPublisher(IHubContext<NotificationHub> hubContext) : IPublisher
{
	public Task PublishAsync<T>(T message) where T : class
	{
		var typeName = typeof(T).Name;
		return hubContext.Clients.All.SendAsync(typeName, message);
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
