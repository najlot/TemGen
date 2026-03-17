using System;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Models;

namespace <# Project.Namespace#>.Client.Data.Repositories;

public interface IUserRepository
{
	Task<UserModel> GetCurrentUserAsync();

	Task<UserListItemModel[]> GetItemsAsync();

	Task<UserModel> GetItemAsync(Guid id);

	Task AddItemAsync(UserModel item);

	Task UpdateItemAsync(UserModel item);

	Task DeleteItemAsync(Guid id);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>