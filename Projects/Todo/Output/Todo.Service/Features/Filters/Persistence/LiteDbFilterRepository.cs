using LiteDB;
using System;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Infrastructure.Persistence.LiteDb;

namespace Todo.Service.Features.Filters.Persistence;

public sealed class LiteDbFilterRepository : IFilterRepository
{
	private readonly ILiteCollection<FilterModel> _collection;

	public LiteDbFilterRepository(LiteDbContext context)
	{
		_collection = context.GetCollection<FilterModel>("Filters");
	}

	public IQueryable<FilterModel> GetAllQueryable()
	{
		return _collection.FindAll().AsQueryable();
	}

	public Task<FilterModel?> Get(Guid id)
	{
		var filter = _collection.FindById(id);
		return Task.FromResult<FilterModel?>(filter);
	}

	public Task Insert(FilterModel model)
	{
		_collection.Insert(model);
		return Task.CompletedTask;
	}

	public Task Update(FilterModel model)
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
