using System.Text.Json;
using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.Service.Services;

public interface IPublisher
{
	Task PublishAsync<T>(T message) where T : notnull;

	Task PublishToUserAsync<T>(string userId, T message) where T : notnull;
}

public class Publisher(MessageHub hub) : IPublisher
{
	public async Task PublishAsync<T>(T message) where T : notnull
	{
		var content = JsonSerializer.Serialize(message);
		await hub.PublishAsync(typeof(T), content).ConfigureAwait(false);
	}

	public async Task PublishToUserAsync<T>(string userId, T message) where T : notnull
	{
		var content = JsonSerializer.Serialize(message);
		await hub.PublishToUserAsync(userId, typeof(T), content).ConfigureAwait(false);
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
