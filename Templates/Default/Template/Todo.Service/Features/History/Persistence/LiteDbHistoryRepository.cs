using LiteDB;
using <# Project.Namespace#>.Service.Infrastructure.Persistence.LiteDb;

namespace <# Project.Namespace#>.Service.Features.History.Persistence;

public class LiteDbHistoryRepository : IHistoryRepository
{
	private readonly ILiteCollection<HistoryModel> _collection;

	public LiteDbHistoryRepository(LiteDbContext context)
	{
		_collection = context.GetCollection<HistoryModel>("HistoryEntries");
	}

	public IQueryable<HistoryModel> GetAllQueryable()
	{
		return _collection.FindAll().AsQueryable();
	}

	public Task<HistoryModel[]> GetHistoryEntries(Guid entityId)
	{
		var items = _collection.FindAll()
			.Where(item => item.EntityId == entityId)
			.OrderByDescending(item => item.TimeStamp)
			.ToArray();

		return Task.FromResult(items);
	}

	public Task DeleteHistoryEntries(Guid entityId)
	{
		var ids = _collection.FindAll().Where(item => item.EntityId == entityId).Select(item => item.Id).ToArray();
		foreach (var id in ids)
		{
			_collection.Delete(id);
		}

		return Task.CompletedTask;
	}

	public Task<HistoryModel?> Get(Guid id)
	{
		var model = _collection.FindById(id);
		return Task.FromResult<HistoryModel?>(model);
	}

	public Task Insert(HistoryModel model)
	{
		_collection.Insert(model);
		return Task.CompletedTask;
	}

	public Task Update(HistoryModel model)
	{
		_collection.Upsert(model);
		return Task.CompletedTask;
	}

	public Task Delete(Guid id)
	{
		_collection.Delete(id);
		return Task.CompletedTask;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>