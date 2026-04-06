using System.Threading.Tasks;

namespace Todo.Service.Infrastructure.Persistence.LiteDb;

public class LiteDbUnitOfWork(LiteDbContext context) : IUnitOfWork
{
	public Task CommitAsync()
	{
		context.Database.Checkpoint();
		return Task.CompletedTask;
	}
}