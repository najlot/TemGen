using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Todo.Service.Configuration;
using Todo.Service.Model;

namespace Todo.Service.Repository;

public class MongoDbNoteRepository : INoteRepository
{
	private readonly IMongoCollection<NoteModel> _collection;
	private readonly MongoDbContext _context;

	public MongoDbNoteRepository(MongoDbContext context)
	{
		_context = context;
		_collection = _context.Database.GetCollection<NoteModel>(nameof(NoteModel)[0..^5]);
	}

	public async IAsyncEnumerable<NoteModel> GetAll()
	{
		var items = await _collection.FindAsync(FilterDefinition<NoteModel>.Empty).ConfigureAwait(false);

		while (await items.MoveNextAsync().ConfigureAwait(false))
		{
			foreach (var item in items.Current)
			{
				yield return item;
			}
		}
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