using System;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Infrastructure.Persistence;

namespace <# Project.Namespace#>.Service.Features.Users;

public interface IUserRepository : IEntityRepository<UserModel>
{
	Task<UserModel?> GetByEmail(string email);
	Task<UserModel?> Get(string username);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>