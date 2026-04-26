using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Todo.Service;
using Todo.Service.Shared.Configuration;

namespace Todo.Service.Features.Filters.Persistence;

public sealed class FileFilterRepository : IFilterRepository
{
	private readonly string _storagePath;

	public FileFilterRepository(FileConfiguration configuration)
	{
		_storagePath = configuration.FiltersPath;
		Directory.CreateDirectory(_storagePath);
	}

	public IQueryable<FilterModel> GetAllQueryable()
	{
		var items = new List<FilterModel>();

		foreach (var path in Directory.GetFiles(_storagePath))
		{
			var bytes = File.ReadAllBytes(path);
			var item = JsonSerializer.Deserialize<FilterModel>(bytes, ServiceJsonSerializer.Options);
			if (item is not null)
			{
				items.Add(item);
			}
		}

		return items.AsQueryable();
	}

	public async Task<FilterModel?> Get(Guid id)
	{
		var path = Path.Combine(_storagePath, id.ToString());
		if (!File.Exists(path))
		{
			return null;
		}

		var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
		return JsonSerializer.Deserialize<FilterModel>(bytes, ServiceJsonSerializer.Options);
	}

	public Task Insert(FilterModel model)
	{
		return Update(model);
	}

	public async Task Update(FilterModel model)
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
