using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Infrastructure.Persistence.MySql;

namespace Todo.Service.Features.Favorites.Persistence;

public sealed class MySqlFavoriteRepository(MySqlDbContext context) : IFavoriteRepository
{
	public IQueryable<FavoriteModel> GetAllQueryable()
	{
		return context
			.Favorites
			.AsNoTracking()
			.AsQueryable();
	}

	public Task<FavoriteModel?> Get(Guid id)
	{
		return context.Favorites.FirstOrDefaultAsync(item => item.Id == id);
	}

	public Task Insert(FavoriteModel model)
	{
		return context.Favorites.AddAsync(model).AsTask();
	}

	public Task Update(FavoriteModel model)
	{
		context.Favorites.Update(model);
		return Task.CompletedTask;
	}

	public async Task Delete(Guid id)
	{
		var model = await context.Favorites.FirstOrDefaultAsync(item => item.Id == id).ConfigureAwait(false);
		if (model != null)
		{
			context.Favorites.Remove(model);
		}
	}
}
