using System;
using System.Threading.Tasks;
using <# Project.Namespace#>.Contracts.Users;

namespace <# Project.Namespace#>.Client.Data.Users;

public interface IUserService
{
	event AsyncEventHandler<UserCreated>? ItemCreated;
	event AsyncEventHandler<UserUpdated>? ItemUpdated;
	event AsyncEventHandler<UserDeleted>? ItemDeleted;

	Task StartEventListener();

	UserModel CreateUser();
	Task AddItemAsync(UserModel item);
	Task<UserListItemModel[]> GetItemsAsync();
	Task<UserModel> GetItemAsync(Guid id);
	Task UpdateItemAsync(UserModel item);
	Task DeleteItemAsync(Guid id);
	Task<UserModel> GetCurrentUserAsync();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>