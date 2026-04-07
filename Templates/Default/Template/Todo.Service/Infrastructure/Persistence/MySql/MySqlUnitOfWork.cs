using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Infrastructure.Persistence;

namespace <# Project.Namespace#>.Service.Infrastructure.Persistence.MySql;

public class MySqlUnitOfWork(MySqlDbContext context) : IUnitOfWork
{
	public Task CommitAsync()
	{
		return context.SaveChangesAsync();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>