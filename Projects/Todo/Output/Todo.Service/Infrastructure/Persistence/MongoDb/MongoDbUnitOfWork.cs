using System.Threading.Tasks;
using Todo.Service.Infrastructure.Persistence;

namespace Todo.Service.Infrastructure.Persistence.MongoDb;

public class MongoDbUnitOfWork : IUnitOfWork
{
	public Task CommitAsync()
	{
		return Task.CompletedTask;
	}
}