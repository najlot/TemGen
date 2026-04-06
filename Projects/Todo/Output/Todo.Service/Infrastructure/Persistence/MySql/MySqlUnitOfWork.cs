using System.Threading.Tasks;
using Todo.Service.Infrastructure.Persistence;

namespace Todo.Service.Infrastructure.Persistence.MySql;

public class MySqlUnitOfWork(MySqlDbContext context) : IUnitOfWork
{
	public Task CommitAsync()
	{
		return context.SaveChangesAsync();
	}
}