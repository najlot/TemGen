using System.Threading.Tasks;

namespace <# Project.Namespace#>.Service.Repository.LiteDbImpl;

public class LiteDbUnitOfWork(LiteDbContext context) : IUnitOfWork
{
	public Task CommitAsync()
	{
		context.Database.Checkpoint();
		return Task.CompletedTask;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>