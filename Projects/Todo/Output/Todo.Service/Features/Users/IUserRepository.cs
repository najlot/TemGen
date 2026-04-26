using System;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Infrastructure.Persistence;

namespace Todo.Service.Features.Users;

public interface IUserRepository : IEntityRepository<UserModel>
{
	Task<UserModel?> GetByEmail(string email);
	Task<UserModel?> Get(string username);
}