using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Contracts.Commands;
using <# Project.Namespace#>.Contracts.ListItems;
using <# Project.Namespace#>.Service.Shared.Results;

namespace <# Project.Namespace#>.Service.Features.Users;

public interface IUserService
{
	Task<Result<User>> GetItem(Guid id);
	Task<Result<User>> GetCurrentUser();
	IAsyncEnumerable<UserListItem> GetItemsForUser();
	Task<UserModel?> GetUserModelFromName(string username);

	Task<Result> CreateUser(CreateUser command);
	Task<Result> UpdateUser(UpdateUser command);
	Task<Result> DeleteUser(Guid id);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>