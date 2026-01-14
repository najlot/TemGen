using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Model;

namespace Todo.Service.Repository;

public class MySqlTodoItemRepository : ITodoItemRepository
{
	private readonly MySqlDbContext _context;

	public MySqlTodoItemRepository(MySqlDbContext context)
	{
		_context = context;
	}

	public IAsyncEnumerable<TodoItemModel> GetAll()
	{
		return _context
			.TodoItems
			.AsNoTracking()
			.AsAsyncEnumerable();
	}

	public IQueryable<TodoItemModel> GetAllQueryable()
	{
		return _context
			.TodoItems
			.AsNoTracking()
			.AsQueryable();
	}

	public async Task<TodoItemModel?> Get(Guid id)
	{
		var e = await _context.TodoItems.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

		if (e == null)
		{
			return null;
		}


		return e;
	}

	public async Task Insert(TodoItemModel model)
	{

		foreach (var entry in model.Checklist)
		{
			entry.Id = 0;
		}

		await _context.TodoItems.AddAsync(model).ConfigureAwait(false);

		await _context.SaveChangesAsync().ConfigureAwait(false);
	}

	public async Task Update(TodoItemModel model)
	{
		_context.TodoItems.Update(model);

		await _context.SaveChangesAsync().ConfigureAwait(false);
	}

	public async Task Delete(Guid id)
	{
		var model = await _context.TodoItems.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

		if (model != null)
		{
			_context.TodoItems.Remove(model);
			await _context.SaveChangesAsync().ConfigureAwait(false);
		}
	}
}