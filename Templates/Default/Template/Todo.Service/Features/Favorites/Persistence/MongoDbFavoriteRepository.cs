using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Infrastructure.Persistence.MongoDb;

namespace <# Project.Namespace#>.Service.Features.Favorites.Persistence;

public sealed class MongoDbFavoriteRepository : IFavoriteRepository
{
	private readonly IMongoCollection<FavoriteModel> _collection;

	public MongoDbFavoriteRepository(MongoDbContext context)
	{
		_collection = context.Database.GetCollection<FavoriteModel>("Favorites");
	}

	public IQueryable<FavoriteModel> GetAllQueryable()
	{
		return _collection.AsQueryable();
	}

	public async Task<FavoriteModel?> Get(Guid id)
	{
		var result = await _collection.FindAsync(item => item.Id == id).ConfigureAwait(false);
		return await result.FirstOrDefaultAsync().ConfigureAwait(false);
	}

	public Task Insert(FavoriteModel model)
	{
		return _collection.InsertOneAsync(model);
	}

	public Task Update(FavoriteModel model)
	{
		return _collection.FindOneAndReplaceAsync(item => item.Id == model.Id, model);
	}

	public Task Delete(Guid id)
	{
		return _collection.DeleteOneAsync(item => item.Id == id);
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>