using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Client.Data.Repositories;

namespace Todo.Client.Data.Services.Implementation;

public sealed class UserService : IUserService
{
	private readonly IUserRepository _repository;

	public UserService(IUserRepository repository)
	{
		_repository = repository;
	}

	public UserModel CreateUser()
	{
		return new UserModel()
		{
			Id = Guid.NewGuid(),
			Username = "",
			EMail = "",
			Password = "",
		};
	}

	public async Task AddItemAsync(UserModel item)
	{
		await _repository.AddItemAsync(item);
	}

	public async Task DeleteItemAsync(Guid id)
	{
		await _repository.DeleteItemAsync(id);
	}

	public async Task<UserModel> GetCurrentUserAsync()
	{
		return await _repository.GetCurrentUserAsync();
	}

	public async Task<UserModel> GetItemAsync(Guid id)
	{
		return await _repository.GetItemAsync(id);
	}

	public async Task<IEnumerable<UserListItemModel>> GetItemsAsync()
	{
		return await _repository.GetItemsAsync();
	}

	public async Task UpdateItemAsync(UserModel item)
	{
		await _repository.UpdateItemAsync(item);
	}

	public void Dispose() => _repository.Dispose();
}