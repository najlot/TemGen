using System.Threading.Tasks;

namespace <# Project.Namespace#>.Service.Infrastructure.Persistence.LiteDb;

public class LiteDbUnitOfWork(LiteDbContext context) : IUnitOfWork
{
	public Task CommitAsync()
	{
		context.Database.Checkpoint();
		return Task.CompletedTask;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>