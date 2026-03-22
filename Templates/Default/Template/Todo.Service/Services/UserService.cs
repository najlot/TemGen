using System.Security.Cryptography;
using System.Text;
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
	IMap map,
	IUserIdProvider userIdProvider,
	IPermissionQueryFilter permissionQueryFilter) : IUserService
{
	public async Task<Result> CreateUser(CreateUser command)
	{
		var username = command.Username.Normalize().ToLower();

		var user = await userRepository.Get(username).ConfigureAwait(false);
		if (user != null)
		{
			return Result.Conflict("User already exists!");
		}

		if (command.Password.Trim().Length < 6)
		{
			return Result.Validation("Password too short!");
		}

		var passwordBytes = Encoding.UTF8.GetBytes(command.Password);
		var passwordHash = SHA256.HashData(passwordBytes);

		var item = map.From(command).To<UserModel>();
		item.Username = username;
		item.PasswordHash = passwordHash;
		await userRepository.Insert(item).ConfigureAwait(false);

		var message = map.From(item).To<UserCreated>();
		await publisher.PublishAsync(message).ConfigureAwait(false);
		return Result.Success();
	}

	public async Task<Result> UpdateUser(UpdateUser command)
	{
		var userId = userIdProvider.GetRequiredUserId();
		var username = command.Username.Normalize().ToLower();

		var item = await userRepository.Get(command.Id).ConfigureAwait(false);

		if (item == null)
		{
			return Result.NotFound("User not found!");
		}

		if (item.Id != userId)
		{
			return Result.Forbidden("You must not modify other users!");
		}

		if (item.Username != username)
		{
			return Result.Validation("Username can not be modified!");
		}

		item.EMail = command.EMail;

		if (!string.IsNullOrWhiteSpace(command.Password))
		{
			if (command.Password.Trim().Length < 6)
			{
				return Result.Validation("Password too short!");
			}

			var passwordBytes = Encoding.UTF8.GetBytes(command.Password);
			item.PasswordHash = SHA256.HashData(passwordBytes);
		}

		await userRepository.Update(item).ConfigureAwait(false);

		var message = map.From(item).To<UserUpdated>();
		await publisher.PublishAsync(message).ConfigureAwait(false);
		return Result.Success();
	}

	public async Task<Result> DeleteUser(Guid id)
	{
		var userId = userIdProvider.GetRequiredUserId();
		var item = await userRepository.Get(id).ConfigureAwait(false);

		if (item == null)
		{
			return Result.NotFound("User not found!");
		}

		if (item.Id != userId)
		{
			return Result.Forbidden("You must not delete other user!");
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
		return Result.Success();
	}

	public async Task<Result<User>> GetItem(Guid id)
	{
		var item = await userRepository.Get(id).ConfigureAwait(false);

		if (item == null)
		{
			return Result<User>.NotFound("User not found!");
		}

		return Result<User>.Success(map.From(item).To<User>());
	}

	public Task<Result<User>> GetCurrentUser()
	{
		var userId = userIdProvider.GetRequiredUserId();
		return GetItem(userId);
	}

	public IAsyncEnumerable<UserListItem> GetItemsForUser()
	{
		var query = userRepository.GetAllQueryable();

		query = query.Where(e => e.DeletedAt == null);
		query = permissionQueryFilter.ApplyReadFilter(query);

		return map.From(query).To<UserListItem>().ToAsyncEnumerable();
	}

	public async Task<UserModel?> GetUserModelFromName(string username)
	{
		username = username.Normalize().ToLower();
		var user = await userRepository.Get(username).ConfigureAwait(false);
		return user;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>