using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Features.Notes;
using Todo.Service.Infrastructure.Persistence.MongoDb;

namespace Todo.Service.Features.Notes.Persistence;

public class MongoDbNoteRepository : INoteRepository
{
	private readonly IMongoCollection<NoteModel> _collection;
	private readonly MongoDbContext _context;

	public MongoDbNoteRepository(MongoDbContext context)
	{
		_context = context;
		_collection = _context.Database.GetCollection<NoteModel>("Note");
	}

	public IQueryable<NoteModel> GetAllQueryable()
	{
		return _collection.AsQueryable();
	}

	public async Task<NoteModel?> Get(Guid id)
	{
		var result = await _collection.FindAsync(item => item.Id == id).ConfigureAwait(false);
		return await result.FirstOrDefaultAsync().ConfigureAwait(false);
	}

	public async Task Insert(NoteModel model)
	{
		await _collection.InsertOneAsync(model).ConfigureAwait(false);
	}

	public async Task Update(NoteModel model)
	{
		await _collection.FindOneAndReplaceAsync(item => item.Id == model.Id, model).ConfigureAwait(false);
	}

	public async Task Delete(Guid id)
	{
		await _collection.DeleteOneAsync(item => item.Id == id).ConfigureAwait(false);
	}
}