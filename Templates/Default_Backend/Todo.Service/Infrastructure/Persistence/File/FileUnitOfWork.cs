using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Infrastructure.Persistence;

namespace <# Project.Namespace#>.Service.Infrastructure.Persistence.File;

public class FileUnitOfWork : IUnitOfWork
{
	public Task CommitAsync()
	{
		return Task.CompletedTask;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>