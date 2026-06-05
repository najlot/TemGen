using System.Text.Json;
using <# Project.Namespace#>.Service;
using <# Project.Namespace#>.Service.Shared.Configuration;

namespace <# Project.Namespace#>.Service.Features.History.Persistence;

public class FileHistoryRepository : IHistoryRepository
{
	private readonly string _storagePath;

	public FileHistoryRepository(FileConfiguration configuration)
	{
		_storagePath = configuration.HistoryPath;
		Directory.CreateDirectory(_storagePath);
	}

	public IQueryable<HistoryModel> GetAllQueryable()
	{
		var items = new List<HistoryModel>();

		foreach (var path in Directory.GetFiles(_storagePath))
		{
			var bytes = File.ReadAllBytes(path);
			var item = JsonSerializer.Deserialize<HistoryModel>(bytes, ServiceJsonSerializer.Options);
			if (item is not null)
			{
				items.Add(item);
			}
		}

		return items.AsQueryable();
	}

	public Task<HistoryModel[]> GetHistoryEntries(Guid entityId)
	{
		var items = GetAllQueryable()
			.Where(item => item.EntityId == entityId)
			.OrderByDescending(item => item.TimeStamp)
			.ToArray();

		return Task.FromResult(items);
	}

	public Task DeleteHistoryEntries(Guid entityId)
	{
		var items = GetAllQueryable().Where(item => item.EntityId == entityId).Select(item => item.Id).ToArray();
		foreach (var id in items)
		{
			File.Delete(Path.Combine(_storagePath, id.ToString()));
		}

		return Task.CompletedTask;
	}

	public async Task<HistoryModel?> Get(Guid id)
	{
		var path = Path.Combine(_storagePath, id.ToString());

		if (!File.Exists(path))
		{
			return null;
		}

		var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
		return JsonSerializer.Deserialize<HistoryModel>(bytes, ServiceJsonSerializer.Options);
	}

	public Task Insert(HistoryModel model)
	{
		return Update(model);
	}

	public async Task Update(HistoryModel model)
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
<#cs SetOutputPathAndSkipOtherDefinitions()#>