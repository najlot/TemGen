using Todo.Contracts;
using Todo.Contracts.Commands;
using Todo.Contracts.ListItems;
using Todo.Service.Shared.Results;

namespace Todo.Service.Features.Users;

public interface IUserService
{
	Task<Result<User>> GetItem(Guid id);
	Task<Result<User>> GetCurrentUser();
	IAsyncEnumerable<UserListItem> GetItemsForUser();
	Task<UserModel?> GetUserModelFromName(string username);

	Task<Result> CreateUser(CreateUser command);
	Task<Result> UpdateUser(UpdateUser command);
	Task<Result> DeleteUser(Guid id);
}