using Microsoft.EntityFrameworkCore;
using <# Project.Namespace#>.Service.Infrastructure.Persistence.MySql;

namespace <# Project.Namespace#>.Service.Features.History.Persistence;

public class MySqlHistoryRepository(MySqlDbContext context) : IHistoryRepository
{
	public IQueryable<HistoryModel> GetAllQueryable()
	{
		return context
			.HistoryEntries
			.AsNoTracking()
			.AsQueryable();
	}

	public Task<HistoryModel[]> GetHistoryEntries(Guid entityId)
	{
		return context.HistoryEntries
			.AsNoTracking()
			.Where(item => item.EntityId == entityId)
			.OrderByDescending(item => item.TimeStamp)
			.ToArrayAsync();
	}

	public async Task DeleteHistoryEntries(Guid entityId)
	{
		var models = await context.HistoryEntries
			.Where(item => item.EntityId == entityId)
			.ToListAsync();
		context.HistoryEntries.RemoveRange(models);
	}

	public Task<HistoryModel?> Get(Guid id)
	{
		return context.HistoryEntries.FirstOrDefaultAsync(item => item.Id == id);
	}

	public Task Insert(HistoryModel model)
	{
		return context.HistoryEntries.AddAsync(model).AsTask();
	}

	public Task Update(HistoryModel model)
	{
		context.HistoryEntries.Update(model);
		return Task.CompletedTask;
	}

	public async Task Delete(Guid id)
	{
		var model = await context.HistoryEntries.FirstOrDefaultAsync(item => item.Id == id).ConfigureAwait(false);
		if (model != null)
		{
			context.HistoryEntries.Remove(model);
		}
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>