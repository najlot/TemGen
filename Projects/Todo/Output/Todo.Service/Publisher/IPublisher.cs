using System.Threading.Tasks;

namespace Todo.Service.Publisher;

public interface IPublisher
{
	Task PublishAsync<T>(T message) where T : class;
}
