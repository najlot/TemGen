using System.Threading.Tasks;

namespace <# Project.Namespace#>.Service.Infrastructure.Persistence;

public interface IUnitOfWork
{
	Task CommitAsync();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>