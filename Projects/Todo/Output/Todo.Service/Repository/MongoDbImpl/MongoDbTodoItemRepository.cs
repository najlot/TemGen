using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Model;
using Todo.Service.Repository;

namespace Todo.Service.Repository.MongoDbImpl;

public class MongoDbTodoItemRepository : ITodoItemRepository
{
	private readonly IMongoCollection<TodoItemModel> _collection;
	private readonly MongoDbContext _context;

	public MongoDbTodoItemRepository(MongoDbContext context)
	{
		_context = context;
		_collection = _context.Database.GetCollection<TodoItemModel>("TodoItem");
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