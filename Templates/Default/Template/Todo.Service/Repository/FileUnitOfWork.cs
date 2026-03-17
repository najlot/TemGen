using System.Threading.Tasks;

namespace <# Project.Namespace#>.Service.Repository;

public class FileUnitOfWork : IUnitOfWork
{
	public Task CommitAsync()
	{
		return Task.CompletedTask;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>