using System;
using System.Threading.Tasks;
using Todo.Client.Data.Models;

namespace Todo.Client.Data.Repositories;

public interface IUserRepository : IDisposable
{
	Task<UserModel> GetCurrentUserAsync();

	Task<UserListItemModel[]> GetItemsAsync();

	Task<UserModel> GetItemAsync(Guid id);

	Task AddItemAsync(UserModel item);

	Task UpdateItemAsync(UserModel item);

	Task DeleteItemAsync(Guid id);
}