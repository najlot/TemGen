using System;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Contracts.Events;

namespace Todo.Client.Data.Services;

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
}