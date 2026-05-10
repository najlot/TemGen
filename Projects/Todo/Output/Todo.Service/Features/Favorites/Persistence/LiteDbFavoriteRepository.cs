using LiteDB;
using System;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Infrastructure.Persistence.LiteDb;

namespace Todo.Service.Features.Favorites.Persistence;

public sealed class LiteDbFavoriteRepository : IFavoriteRepository
{
	private readonly ILiteCollection<FavoriteModel> _collection;

	public LiteDbFavoriteRepository(LiteDbContext context)
	{
		_collection = context.GetCollection<FavoriteModel>("Favorites");
	}

	public IQueryable<FavoriteModel> GetAllQueryable()
	{
		return _collection.FindAll().AsQueryable();
	}

	public Task<FavoriteModel?> Get(Guid id)
	{
		var favorite = _collection.FindById(id);
		return Task.FromResult<FavoriteModel?>(favorite);
	}

	public Task Insert(FavoriteModel model)
	{
		_collection.Insert(model);
		return Task.CompletedTask;
	}

	public Task Update(FavoriteModel model)
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
