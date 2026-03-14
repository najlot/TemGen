using System.Threading.Tasks;

namespace Todo.Service.Repository;

public class MongoDbUnitOfWork : IUnitOfWork
{
	public Task CommitAsync()
	{
		return Task.CompletedTask;
	}
}