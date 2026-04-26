using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Todo.Service.Features.Users;
using Todo.Service.Infrastructure.Persistence.MongoDb;

namespace Todo.Service.Features.Users.Persistence;

public class MongoDbUserRepository : IUserRepository
{
	private readonly IMongoCollection<UserModel> _collection;
	private readonly MongoDbContext _context;

	public MongoDbUserRepository(MongoDbContext context)
	{
		_context = context;
		_collection = _context.Database.GetCollection<UserModel>("User");
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

	public async Task<UserModel?> GetByEmail(string email)
	{
		var pattern = $"^{Regex.Escape(email.Trim())}$";
		var filter = Builders<UserModel>.Filter.And(
			Builders<UserModel>.Filter.Eq(item => item.DeletedAt, (DateTime?)null),
			Builders<UserModel>.Filter.Regex(item => item.EMail, new BsonRegularExpression(pattern, "i")));

		var result = await _collection.FindAsync(filter).ConfigureAwait(false);
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
}