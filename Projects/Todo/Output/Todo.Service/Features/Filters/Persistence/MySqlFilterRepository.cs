using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Infrastructure.Persistence.MySql;

namespace Todo.Service.Features.Filters.Persistence;

public sealed class MySqlFilterRepository(MySqlDbContext context) : IFilterRepository
{
	public IQueryable<FilterModel> GetAllQueryable()
	{
		return context
			.Filters
			.AsNoTracking()
			.AsQueryable();
	}

	public Task<FilterModel?> Get(Guid id)
	{
		return context.Filters.FirstOrDefaultAsync(item => item.Id == id);
	}

	public Task Insert(FilterModel model)
	{
		return context.Filters.AddAsync(model).AsTask();
	}

	public Task Update(FilterModel model)
	{
		context.Filters.Update(model);
		return Task.CompletedTask;
	}

	public async Task Delete(Guid id)
	{
		var model = await context.Filters.FirstOrDefaultAsync(item => item.Id == id).ConfigureAwait(false);
		if (model != null)
		{
			context.Filters.Remove(model);
		}
	}
}
