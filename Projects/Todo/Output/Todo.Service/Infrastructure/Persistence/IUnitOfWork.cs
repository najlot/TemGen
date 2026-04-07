using System.Threading.Tasks;

namespace Todo.Service.Infrastructure.Persistence;

public interface IUnitOfWork
{
	Task CommitAsync();
}