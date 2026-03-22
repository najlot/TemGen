using System.Threading.Tasks;
using Todo.Service.Repository;

namespace Todo.Service.Repository.MongoDbImpl;

public class MongoDbUnitOfWork : IUnitOfWork
{
	public Task CommitAsync()
	{
		return Task.CompletedTask;
	}
}