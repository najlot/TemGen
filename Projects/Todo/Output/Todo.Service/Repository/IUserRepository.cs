using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Model;

namespace Todo.Service.Repository;

public interface IUserRepository
{
	IAsyncEnumerable<UserModel> GetAll();

	IQueryable<UserModel> GetAllQueryable();

	Task<UserModel?> Get(Guid id);

	Task<UserModel?> Get(string username);

	Task Insert(UserModel model);

	Task Update(UserModel model);

	Task Delete(Guid id);
}