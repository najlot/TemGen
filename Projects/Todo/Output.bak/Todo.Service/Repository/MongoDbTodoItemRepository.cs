using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Todo.Service.Configuration;
using Todo.Service.Model;

namespace Todo.Service.Repository;

public class MongoDbTodoItemRepository : ITodoItemRepository
{
	private readonly IMongoCollection<TodoItemModel> _collection;
	private readonly MongoDbContext _context;

	public MongoDbTodoItemRepository(MongoDbContext context)
	{
		_context = context;
		_collection = _context.Database.GetCollection<TodoItemModel>(nameof(TodoItemModel)[0..^5]);
	}

	public async IAsyncEnumerable<TodoItemModel> GetAll()
	{
		var items = await _collection.FindAsync(FilterDefinition<TodoItemModel>.Empty).ConfigureAwait(false);

		while (await items.MoveNextAsync().ConfigureAwait(false))
		{
			foreach (var item in items.Current)
			{
				yield return item;
			}
		}
	}

	public IQueryable<TodoItemModel> GetAllQueryable()
	{
		return _collection.AsQueryable();
	}

	public async Task<TodoItemModel?> Get(Guid id)
	{
		var result = await _collection.FindAsync(item => item.Id == id).ConfigureAwait(false);
		return await result.FirstOrDefaultAsync().ConfigureAwait(false);
	}

	public async Task Insert(TodoItemModel model)
	{
		await _collection.InsertOneAsync(model).ConfigureAwait(false);
	}

	public async Task Update(TodoItemModel model)
	{
		await _collection.FindOneAndReplaceAsync(item => item.Id == model.Id, model).ConfigureAwait(false);
	}

	public async Task Delete(Guid id)
	{
		await _collection.DeleteOneAsync(item => item.Id == id).ConfigureAwait(false);
	}
}