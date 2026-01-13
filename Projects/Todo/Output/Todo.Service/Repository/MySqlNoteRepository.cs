using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Model;

namespace Todo.Service.Repository;

public class MySqlNoteRepository : INoteRepository
{
	private readonly MySqlDbContext _context;

	public MySqlNoteRepository(MySqlDbContext context)
	{
		_context = context;
	}

	public IAsyncEnumerable<NoteModel> GetAll()
	{
		return _context
			.Notes
			.AsNoTracking()
			.AsAsyncEnumerable();
	}

	public IQueryable<NoteModel> GetAllQueryable()
	{
		return _context
			.Notes
			.AsNoTracking()
			.AsQueryable();
	}

	public async Task<NoteModel?> Get(Guid id)
	{
		var e = await _context.Notes.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

		if (e == null)
		{
			return null;
		}


		return e;
	}

	public async Task Insert(NoteModel model)
	{
		await _context.Notes.AddAsync(model).ConfigureAwait(false);

		await _context.SaveChangesAsync().ConfigureAwait(false);
	}

	public async Task Update(NoteModel model)
	{
		_context.Notes.Update(model);

		await _context.SaveChangesAsync().ConfigureAwait(false);
	}

	public async Task Delete(Guid id)
	{
		var model = await _context.Notes.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

		if (model != null)
		{
			_context.Notes.Remove(model);
			await _context.SaveChangesAsync().ConfigureAwait(false);
		}
	}
}