using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Repository;

namespace <# Project.Namespace#>.Service.Repository.MongoDbImpl;

public class MongoDbUnitOfWork : IUnitOfWork
{
	public Task CommitAsync()
	{
		return Task.CompletedTask;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>