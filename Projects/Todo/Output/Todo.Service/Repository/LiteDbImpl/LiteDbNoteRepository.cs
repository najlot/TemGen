using LiteDB;
using System;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Model;

namespace Todo.Service.Repository.LiteDbImpl;

public class LiteDbNoteRepository : INoteRepository
{
	private readonly ILiteCollection<NoteModel> _collection;

	public LiteDbNoteRepository(LiteDbContext context)
	{
		_collection = context.GetCollection<NoteModel>("Notes");
	}

	public IQueryable<NoteModel> GetAllQueryable()
	{
		return _collection.FindAll().AsQueryable();
	}

	public Task<NoteModel?> Get(Guid id)
	{
		var model = _collection.FindById(id);
		return Task.FromResult<NoteModel?>(model);
	}

	public Task Insert(NoteModel model)
	{
		_collection.Insert(model);
		return Task.CompletedTask;
	}

	public Task Update(NoteModel model)
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