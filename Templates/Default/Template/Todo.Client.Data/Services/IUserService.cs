using System;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.Contracts.Events;

namespace <# Project.Namespace#>.Client.Data.Services;

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