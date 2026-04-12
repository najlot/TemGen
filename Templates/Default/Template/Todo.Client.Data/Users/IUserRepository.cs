using System;
using System.Threading.Tasks;

namespace <# Project.Namespace#>.Client.Data.Users;

public interface IUserRepository
{
	Task<UserModel> GetCurrentUserAsync();

	Task<UserListItemModel[]> GetItemsAsync();

	Task<UserModel> GetItemAsync(Guid id);

	Task AddItemAsync(UserModel item);

	Task UpdateItemAsync(UserModel item);

	Task DeleteItemAsync(Guid id);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>