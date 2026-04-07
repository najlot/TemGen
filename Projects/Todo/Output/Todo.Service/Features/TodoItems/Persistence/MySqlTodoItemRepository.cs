using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Features.TodoItems;
using Todo.Service.Infrastructure.Persistence.MySql;

namespace Todo.Service.Features.TodoItems.Persistence;

public class MySqlTodoItemRepository(MySqlDbContext context) : ITodoItemRepository
{
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