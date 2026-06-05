using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Infrastructure.Persistence;

namespace <# Project.Namespace#>.Service.Infrastructure.Persistence.MongoDb;

public class MongoDbUnitOfWork : IUnitOfWork
{
	public Task CommitAsync()
	{
		return Task.CompletedTask;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>