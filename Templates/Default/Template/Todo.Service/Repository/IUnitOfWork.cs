using System.Threading.Tasks;

namespace <# Project.Namespace#>.Service.Repository;

public interface IUnitOfWork
{
	Task CommitAsync();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>