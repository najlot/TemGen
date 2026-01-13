using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Service.Configuration;
using <#cs Write(Project.Namespace)#>.Service.Model;

namespace <#cs Write(Project.Namespace)#>.Service.Repository;

public class FileUserRepository : IUserRepository
{
	private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
	private readonly string _storagePath;

	public FileUserRepository(FileConfiguration configuration)
	{
		_storagePath = configuration.UsersPath;
		Directory.CreateDirectory(_storagePath);
	}

	public async IAsyncEnumerable<UserModel> GetAll()
	{
		foreach (var path in Directory.GetFiles(_storagePath))
		{
			var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
			var text = Encoding.UTF8.GetString(bytes);
			var item = JsonSerializer.Deserialize<UserModel>(text, _options);
			if (item is not null)
			{
				yield return item;
			}
		}
	}

	public async Task<UserModel?> Get(Guid id)
	{
		var path = Path.Combine(_storagePath, id.ToString());

		if (!File.Exists(path))
		{
			return null;
		}

		var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
		var item = JsonSerializer.Deserialize<UserModel>(bytes, _options);

		return item;
	}

	public async Task<UserModel?> Get(string username)
	{
		foreach (var path in Directory.GetFiles(_storagePath))
		{
			var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
			var text = Encoding.UTF8.GetString(bytes);
			var item = JsonSerializer.Deserialize<UserModel>(text, _options);

			if (item is not null && item.IsActive && item.Username == username)
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
		var bytes = JsonSerializer.SerializeToUtf8Bytes(model);
		await File.WriteAllBytesAsync(path, bytes).ConfigureAwait(false);
	}

	public Task Delete(Guid id)
	{
		var path = Path.Combine(_storagePath, id.ToString());
		File.Delete(path);
		return Task.CompletedTask;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>