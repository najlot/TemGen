using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Model;

namespace <# Project.Namespace#>.Service.Repository;

public class MongoDbUserRepository : IUserRepository
{
	private readonly IMongoCollection<UserModel> _collection;
	private readonly MongoDbContext _context;

	public MongoDbUserRepository(MongoDbContext context)
	{
		_context = context;
		_collection = _context.Database.GetCollection<UserModel>("User");
	}

	public async IAsyncEnumerable<UserModel> GetAll()
	{
		var items = await _collection.FindAsync(FilterDefinition<UserModel>.Empty).ConfigureAwait(false);

		while (await items.MoveNextAsync().ConfigureAwait(false))
		{
			foreach (var item in items.Current)
			{
				yield return item;
			}
		}
	}

	public IQueryable<UserModel> GetAllQueryable()
	{
		return _collection.AsQueryable();
	}

	public async Task<UserModel?> Get(Guid id)
	{
		var result = await _collection.FindAsync(item => item.Id == id).ConfigureAwait(false);
		return await result.FirstOrDefaultAsync().ConfigureAwait(false);
	}

	public async Task<UserModel?> Get(string username)
	{
		var result = await _collection.FindAsync(item => item.Username == username && item.DeletedAt == null).ConfigureAwait(false);
		return await result.FirstOrDefaultAsync().ConfigureAwait(false);
	}

	public async Task Insert(UserModel model)
	{
		await _collection.InsertOneAsync(model).ConfigureAwait(false);
	}

	public async Task Update(UserModel model)
	{
		await _collection.FindOneAndReplaceAsync(item => item.Id == model.Id, model).ConfigureAwait(false);
	}

	public async Task Delete(Guid id)
	{
		await _collection.DeleteOneAsync(item => item.Id == id).ConfigureAwait(false);
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>