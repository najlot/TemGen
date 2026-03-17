using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Najlot.Map;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Service.Model;
using <# Project.Namespace#>.Service.Repository;
using <# Project.Namespace#>.Contracts.Commands;
using <# Project.Namespace#>.Contracts.Events;
using <# Project.Namespace#>.Contracts.ListItems;

namespace <# Project.Namespace#>.Service.Services;

public class UserService(
	IUserRepository userRepository,
	IPublisher publisher,
	IMap map) : IUserService
{
	public async Task CreateUser(CreateUser command, Guid userId)
	{
		var username = command.Username.Normalize().ToLower();

		var user = await userRepository.Get(username).ConfigureAwait(false);
		if (user != null)
		{
			throw new InvalidOperationException("User already exists!");
		}

		if (command.Password.Trim().Length < 6)
		{
			throw new InvalidOperationException("Password too short!");
		}

		var passwordBytes = Encoding.UTF8.GetBytes(command.Password);
		var passwordHash = SHA256.HashData(passwordBytes);

		var item = map.From(command).To<UserModel>();
		item.Username = username;
		item.PasswordHash = passwordHash;
		await userRepository.Insert(item).ConfigureAwait(false);

		var message = map.From(item).To<UserCreated>();
		await publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task UpdateUser(UpdateUser command, Guid userId)
	{
		var username = command.Username.Normalize().ToLower();

		var item = await userRepository.Get(command.Id).ConfigureAwait(false);

		if (item == null)
		{
			throw new InvalidOperationException("User not found!");
		}

		if (item.Id != userId)
		{
			throw new InvalidOperationException("You must not modify other users!");
		}

		if (item.Username != username)
		{
			throw new InvalidOperationException("Username can not be modified!");
		}

		item.EMail = command.EMail;

		if (!string.IsNullOrWhiteSpace(command.Password))
		{
			if (command.Password.Trim().Length < 6)
			{
				throw new InvalidOperationException("Password too short!");
			}

			var passwordBytes = Encoding.UTF8.GetBytes(command.Password);
			item.PasswordHash = SHA256.HashData(passwordBytes);
		}

		await userRepository.Update(item).ConfigureAwait(false);

		var message = map.From(item).To<UserUpdated>();
		await publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task DeleteUser(Guid id, Guid userId)
	{
		var item = await userRepository.Get(id).ConfigureAwait(false);

		if (item == null)
		{
			throw new InvalidOperationException("User not found!");
		}

		if (item.Id != userId)
		{
			throw new InvalidOperationException("You must not delete other user!");
		}

		if (item.DeletedAt == null)
		{
			item.DeletedAt = DateTime.UtcNow;
			await userRepository.Update(item).ConfigureAwait(false);
		}
		else
		{
			await userRepository.Delete(id).ConfigureAwait(false);
		}

		var message = new UserDeleted(id);
		await publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task<User?> GetItem(Guid id)
	{
		var item = await userRepository.Get(id).ConfigureAwait(false);
		return map.FromNullable(item)?.To<User>();
	}

	public IAsyncEnumerable<UserListItem> GetItemsForUser(Guid userId)
	{
		var query = userRepository.GetAllQueryable();

		query = query.Where(e => e.DeletedAt == null);

		return map.From(query).To<UserListItem>().ToAsyncEnumerable();
	}

	public async Task<UserModel?> GetUserModelFromName(string username)
	{
		username = username.Normalize().ToLower();
		var user = await userRepository.Get(username).ConfigureAwait(false);
		return user;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>