using MongoDB.Driver;
using <# Project.Namespace#>.Service.Infrastructure.Persistence.MongoDb;

namespace <# Project.Namespace#>.Service.Features.History.Persistence;

public class MongoDbHistoryRepository : IHistoryRepository
{
	private readonly IMongoCollection<HistoryModel> _collection;

	public MongoDbHistoryRepository(MongoDbContext context)
	{
		_collection = context.Database.GetCollection<HistoryModel>("HistoryEntries");
	}

	public IQueryable<HistoryModel> GetAllQueryable()
	{
		return _collection.AsQueryable();
	}

	public async Task<HistoryModel[]> GetHistoryEntries(Guid entityId)
	{
		var items = await _collection
			.Find(item => item.EntityId == entityId)
			.SortByDescending(item => item.TimeStamp)
			.ToListAsync()
			.ConfigureAwait(false);

		return items.ToArray();
	}

	public Task DeleteHistoryEntries(Guid entityId)
	{
		return _collection.DeleteManyAsync(item => item.EntityId == entityId);
	}

	public async Task<HistoryModel?> Get(Guid id)
	{
		var result = await _collection.FindAsync(item => item.Id == id).ConfigureAwait(false);
		return await result.FirstOrDefaultAsync().ConfigureAwait(false);
	}

	public Task Insert(HistoryModel model)
	{
		return _collection.InsertOneAsync(model);
	}

	public Task Update(HistoryModel model)
	{
		return _collection.FindOneAndReplaceAsync(item => item.Id == model.Id, model);
	}

	public Task Delete(Guid id)
	{
		return _collection.DeleteOneAsync(item => item.Id == id);
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>