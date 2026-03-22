using System.Threading.Tasks;
using Todo.Service.Repository;

namespace Todo.Service.Repository.MySqlImpl;

public class MySqlUnitOfWork(MySqlDbContext context) : IUnitOfWork
{
	public Task CommitAsync()
	{
		return context.SaveChangesAsync();
	}
}