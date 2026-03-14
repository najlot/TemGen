using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.Service.Repository;

public class MySqlUnitOfWork(MySqlDbContext context) : IUnitOfWork
{
	public Task CommitAsync()
	{
		return context.SaveChangesAsync();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>