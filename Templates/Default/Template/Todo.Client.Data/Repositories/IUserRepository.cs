using System;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Repositories;

public interface IUserRepository : IDisposable
{
	Task<UserModel> GetCurrentUserAsync();

	Task<UserListItemModel[]> GetItemsAsync();

	Task<UserModel> GetItemAsync(Guid id);

	Task AddItemAsync(UserModel item);

	Task UpdateItemAsync(UserModel item);

	Task DeleteItemAsync(Guid id);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>