using Cosei.Client.Base;
using System.Threading.Tasks;

namespace Todo.ClientBase.ProfileHandler;

public interface ILocalSubscriber : ISubscriber
{
	Task SendAsync<T>(T message) where T : class;
}