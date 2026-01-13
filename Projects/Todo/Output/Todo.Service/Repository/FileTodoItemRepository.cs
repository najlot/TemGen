using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Todo.Service.Configuration;
using Todo.Service.Model;

namespace Todo.Service.Repository;

public class FileTodoItemRepository : ITodoItemRepository
{
	private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
	private readonly string _storagePath;

	public FileTodoItemRepository(FileConfiguration configuration)
	{
		_storagePath = configuration.TodoItemsPath;
		Directory.CreateDirectory(_storagePath);
	}

	public async IAsyncEnumerable<TodoItemModel> GetAll()
	{
		foreach (var path in Directory.GetFiles(_storagePath))
		{
			var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
			var text = Encoding.UTF8.GetString(bytes);
			var item = JsonSerializer.Deserialize<TodoItemModel>(text, _options);
			if (item is not null)
			{
				yield return item;
			}
		}
	}

	public IQueryable<TodoItemModel> GetAllQueryable()
	{
		var items = new List<TodoItemModel>();

		foreach (var path in Directory.GetFiles(_storagePath))
		{
			var bytes = File.ReadAllBytes(path);
			var text = Encoding.UTF8.GetString(bytes);
			var item = JsonSerializer.Deserialize<TodoItemModel>(text, _options);
			if (item is not null)
			{
				items.Add(item);
			}
		}

		return items.AsQueryable();
	}

	public async Task<TodoItemModel?> Get(Guid id)
	{
		var path = Path.Combine(_storagePath, id.ToString());

		if (!File.Exists(path))
		{
			return null;
		}

		var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
		var item = JsonSerializer.Deserialize<TodoItemModel>(bytes, _options);

		return item;
	}

	public async Task Insert(TodoItemModel model)
	{
		await Update(model).ConfigureAwait(false);
	}

	public async Task Update(TodoItemModel model)
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
}