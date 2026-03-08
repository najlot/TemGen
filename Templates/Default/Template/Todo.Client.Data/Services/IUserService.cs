using System;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Contracts.Events;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Services;

public interface IUserService
{
	event AsyncEventHandler<UserCreated>? OnItemCreated;
	event AsyncEventHandler<UserUpdated>? OnItemUpdated;
	event AsyncEventHandler<UserDeleted>? OnItemDeleted;

	Task StartEventListener();

	UserModel CreateUser();
	Task AddItemAsync(UserModel item);
	Task<UserListItemModel[]> GetItemsAsync();
	Task<UserModel> GetItemAsync(Guid id);
	Task UpdateItemAsync(UserModel item);
	Task DeleteItemAsync(Guid id);
	Task<UserModel> GetCurrentUserAsync();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>