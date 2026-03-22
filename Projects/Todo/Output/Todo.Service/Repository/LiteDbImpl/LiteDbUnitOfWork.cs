using System.Threading.Tasks;

namespace Todo.Service.Repository.LiteDbImpl;

public class LiteDbUnitOfWork(LiteDbContext context) : IUnitOfWork
{
	public Task CommitAsync()
	{
		context.Database.Checkpoint();
		return Task.CompletedTask;
	}
}