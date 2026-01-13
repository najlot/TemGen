using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Services;

public interface IUserService : IDisposable
{
	UserModel CreateUser();
	Task AddItemAsync(UserModel item);
	Task<IEnumerable<UserListItemModel>> GetItemsAsync();
	Task<UserModel> GetItemAsync(Guid id);
	Task UpdateItemAsync(UserModel item);
	Task DeleteItemAsync(Guid id);
	Task<UserModel> GetCurrentUserAsync();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>