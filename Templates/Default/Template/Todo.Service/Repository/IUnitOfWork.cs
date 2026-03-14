using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.Service.Repository;

public interface IUnitOfWork
{
	Task CommitAsync();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>