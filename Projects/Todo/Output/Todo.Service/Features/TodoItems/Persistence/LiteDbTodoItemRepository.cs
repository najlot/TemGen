using LiteDB;
using System;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Features.TodoItems;
using Todo.Service.Infrastructure.Persistence.LiteDb;

namespace Todo.Service.Features.TodoItems.Persistence;

public class LiteDbTodoItemRepository : ITodoItemRepository
{
	private readonly ILiteCollection<TodoItemModel> _collection;

	public LiteDbTodoItemRepository(LiteDbContext context)
	{
		_collection = context.GetCollection<TodoItemModel>("TodoItems");
	}

	public IQueryable<TodoItemModel> GetAllQueryable()
	{
		return _collection.FindAll().AsQueryable();
	}

	public Task<TodoItemModel?> Get(Guid id)
	{
		var model = _collection.FindById(id);
		return Task.FromResult<TodoItemModel?>(model);
	}

	public Task Insert(TodoItemModel model)
	{
		_collection.Insert(model);
		return Task.CompletedTask;
	}

	public Task Update(TodoItemModel model)
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