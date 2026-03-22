using System.Threading.Tasks;
using Todo.Service.Repository;

namespace Todo.Service.Repository.FileImpl;

public class FileUnitOfWork : IUnitOfWork
{
	public Task CommitAsync()
	{
		return Task.CompletedTask;
	}
}