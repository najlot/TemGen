using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Infrastructure.Persistence.MongoDb;

namespace <# Project.Namespace#>.Service.Features.Filters.Persistence;

public sealed class MongoDbFilterRepository : IFilterRepository
{
	private readonly IMongoCollection<FilterModel> _collection;

	public MongoDbFilterRepository(MongoDbContext context)
	{
		_collection = context.Database.GetCollection<FilterModel>("Filters");
	}

	public IQueryable<FilterModel> GetAllQueryable()
	{
		return _collection.AsQueryable();
	}

	public async Task<FilterModel?> Get(Guid id)
	{
		var result = await _collection.FindAsync(item => item.Id == id).ConfigureAwait(false);
		return await result.FirstOrDefaultAsync().ConfigureAwait(false);
	}

	public Task Insert(FilterModel model)
	{
		return _collection.InsertOneAsync(model);
	}

	public Task Update(FilterModel model)
	{
		return _collection.FindOneAndReplaceAsync(item => item.Id == model.Id, model);
	}

	public Task Delete(Guid id)
	{
		return _collection.DeleteOneAsync(item => item.Id == id);
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>