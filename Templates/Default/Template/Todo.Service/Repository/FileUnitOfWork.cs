using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.Service.Repository;

public class FileUnitOfWork : IUnitOfWork
{
	public Task CommitAsync()
	{
		return Task.CompletedTask;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>