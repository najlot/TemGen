using System.Threading.Tasks;

namespace Todo.Service.Repository;

public class FileUnitOfWork : IUnitOfWork
{
	public Task CommitAsync()
	{
		return Task.CompletedTask;
	}
}