using System;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Model;

namespace <# Project.Namespace#>.Service.Repository;

public interface IUserRepository : IEntityRepository<UserModel>
{
	Task<UserModel?> Get(string username);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>