using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Repository;

namespace <# Project.Namespace#>.Service.Repository.MySqlImpl;

public class MySqlUnitOfWork(MySqlDbContext context) : IUnitOfWork
{
	public Task CommitAsync()
	{
		return context.SaveChangesAsync();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>