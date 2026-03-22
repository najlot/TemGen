using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Model;
using Todo.Service.Repository;

namespace Todo.Service.Repository.MySqlImpl;

public class MySqlNoteRepository(MySqlDbContext context) : INoteRepository
{
	public IQueryable<NoteModel> GetAllQueryable()
	{
		return context
			.Notes
			.AsNoTracking()
			.AsQueryable();
	}

	public async Task<NoteModel?> Get(Guid id)
	{
		var e = await context.Notes.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

		if (e == null)
		{
			return null;
		}

		return e;
	}

	public async Task Insert(NoteModel model)
	{
		await context.Notes.AddAsync(model).ConfigureAwait(false);
	}

	public Task Update(NoteModel model)
	{
		context.Notes.Update(model);
		return Task.CompletedTask;
	}

	public async Task Delete(Guid id)
	{
		var model = await context.Notes.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

		if (model != null)
		{
			context.Notes.Remove(model);
		}
	}
}