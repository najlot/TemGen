using System;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Model;

namespace Todo.Service.Repository;

public interface IUserRepository : IEntityRepository<UserModel>
{
	Task<UserModel?> Get(string username);
}