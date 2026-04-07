using System.Threading.Tasks;
using Todo.Service.Infrastructure.Persistence;

namespace Todo.Service.Infrastructure.Persistence.File;

public class FileUnitOfWork : IUnitOfWork
{
	public Task CommitAsync()
	{
		return Task.CompletedTask;
	}
}