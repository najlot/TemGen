using System.Threading.Tasks;

namespace Todo.Service.Repository;

public class MySqlUnitOfWork(MySqlDbContext context) : IUnitOfWork
{
	public Task CommitAsync()
	{
		return context.SaveChangesAsync();
	}
}