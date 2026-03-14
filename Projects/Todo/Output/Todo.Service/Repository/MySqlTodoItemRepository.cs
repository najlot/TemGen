using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Model;

namespace Todo.Service.Repository;

public class MySqlTodoItemRepository(MySqlDbContext context) : ITodoItemRepository
{
	public IAsyncEnumerable<TodoItemModel> GetAll()
	{
		return context
			.TodoItems
			.AsNoTracking()
			.AsAsyncEnumerable();
	}

	public IQueryable<TodoItemModel> GetAllQueryable()
	{
		return context
			.TodoItems
			.AsNoTracking()
			.AsQueryable();
	}

	public async Task<TodoItemModel?> Get(Guid id)
	{
		var e = await context.TodoItems.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

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

		await context.TodoItems.AddAsync(model).ConfigureAwait(false);
	}

	public Task Update(TodoItemModel model)
	{
		context.TodoItems.Update(model);
		return Task.CompletedTask;
	}

	public async Task Delete(Guid id)
	{
		var model = await context.TodoItems.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

		if (model != null)
		{
			context.TodoItems.Remove(model);
		}
	}
}