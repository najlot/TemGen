using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Contracts.Commands;
using <# Project.Namespace#>.Contracts.ListItems;
using <# Project.Namespace#>.Service.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace <# Project.Namespace#>.Service.Services;

public interface IUserService
{
	Task<User?> GetItem(Guid id);
	IAsyncEnumerable<UserListItem> GetItemsForUser(Guid userId);
	Task<UserModel?> GetUserModelFromName(string username);

	Task CreateUser(CreateUser command, Guid userId);
	Task UpdateUser(UpdateUser command, Guid userId);
	Task DeleteUser(Guid id, Guid userId);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>