using Todo.Contracts;
using Todo.Contracts.Commands;
using Todo.Contracts.ListItems;
using Todo.Service.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Todo.Service.Services;

public interface IUserService
{
	Task<User?> GetItem(Guid id);
	IAsyncEnumerable<UserListItem> GetItemsForUser(Guid userId);
	Task<UserModel?> GetUserModelFromName(string username);

	Task CreateUser(CreateUser command, Guid userId);
	Task UpdateUser(UpdateUser command, Guid userId);
	Task DeleteUser(Guid id, Guid userId);
}