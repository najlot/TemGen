using System.Threading.Tasks;

namespace Todo.Service.Repository;

public interface IUnitOfWork
{
	Task CommitAsync();
}