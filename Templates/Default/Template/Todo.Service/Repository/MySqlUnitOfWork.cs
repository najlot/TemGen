using System.Threading.Tasks;

namespace <# Project.Namespace#>.Service.Repository;

public class MySqlUnitOfWork(MySqlDbContext context) : IUnitOfWork
{
	public Task CommitAsync()
	{
		return context.SaveChangesAsync();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>