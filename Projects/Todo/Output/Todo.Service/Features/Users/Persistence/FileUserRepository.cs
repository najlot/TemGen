using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Todo.Service.Serialization;
using Todo.Service.Shared.Configuration;
using Todo.Service.Features.Users;


namespace Todo.Service.Features.Users.Persistence;

public class FileUserRepository : IUserRepository
{
	private readonly string _storagePath;

	public FileUserRepository(FileConfiguration configuration)
	{
		_storagePath = configuration.UsersPath;
		Directory.CreateDirectory(_storagePath);
	}

	public IQueryable<UserModel> GetAllQueryable()
	{
		var items = new List<UserModel>();

		foreach (var path in Directory.GetFiles(_storagePath))
		{
			var bytes = File.ReadAllBytes(path);
			var item = JsonSerializer.Deserialize<UserModel>(bytes, ServiceJsonSerializer.Options);
			if (item is not null)
			{
				items.Add(item);
			}
		}

		return items.AsQueryable();
	}

	public async Task<UserModel?> Get(Guid id)
	{
		var path = Path.Combine(_storagePath, id.ToString());

		if (!File.Exists(path))
		{
			return null;
		}

		var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
		var item = JsonSerializer.Deserialize<UserModel>(bytes, ServiceJsonSerializer.Options);

		return item;
	}

	public async Task<UserModel?> Get(string username)
	{
		foreach (var path in Directory.GetFiles(_storagePath))
		{
			var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
			var item = JsonSerializer.Deserialize<UserModel>(bytes, ServiceJsonSerializer.Options);

			if (item is not null && item.DeletedAt == null && item.Username == username)
			{
				return item;
			}
		}

		return null;
	}

	public async Task Insert(UserModel model)
	{
		await Update(model).ConfigureAwait(false);
	}

	public async Task Update(UserModel model)
	{
		var path = Path.Combine(_storagePath, model.Id.ToString());
		var bytes = JsonSerializer.SerializeToUtf8Bytes(model, ServiceJsonSerializer.Options);
		await File.WriteAllBytesAsync(path, bytes).ConfigureAwait(false);
	}

	public Task Delete(Guid id)
	{
		var path = Path.Combine(_storagePath, id.ToString());
		File.Delete(path);
		return Task.CompletedTask;
	}
}