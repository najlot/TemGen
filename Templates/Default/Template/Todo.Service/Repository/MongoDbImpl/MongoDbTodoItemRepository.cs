using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Model;
using <# Project.Namespace#>.Service.Repository;

namespace <# Project.Namespace#>.Service.Repository.MongoDbImpl;

public class MongoDb<# Definition.Name#>Repository : I<# Definition.Name#>Repository
{
	private readonly IMongoCollection<<# Definition.Name#>Model> _collection;
	private readonly MongoDbContext _context;

	public MongoDb<# Definition.Name#>Repository(MongoDbContext context)
	{
		_context = context;
		_collection = _context.Database.GetCollection<<# Definition.Name#>Model>("<# Definition.Name#>");
	}

	public IQueryable<<# Definition.Name#>Model> GetAllQueryable()
	{
		return _collection.AsQueryable();
	}

	public async Task<<# Definition.Name#>Model?> Get(Guid id)
	{
		var result = await _collection.FindAsync(item => item.Id == id).ConfigureAwait(false);
		return await result.FirstOrDefaultAsync().ConfigureAwait(false);
	}

	public async Task Insert(<# Definition.Name#>Model model)
	{
		await _collection.InsertOneAsync(model).ConfigureAwait(false);
	}

	public async Task Update(<# Definition.Name#>Model model)
	{
		await _collection.FindOneAndReplaceAsync(item => item.Id == model.Id, model).ConfigureAwait(false);
	}

	public async Task Delete(Guid id)
	{
		await _collection.DeleteOneAsync(item => item.Id == id).ConfigureAwait(false);
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>