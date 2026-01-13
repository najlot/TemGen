using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Service.Configuration;
using <#cs Write(Project.Namespace)#>.Service.Model;

namespace <#cs Write(Project.Namespace)#>.Service.Repository;

public class MongoDb<#cs Write(Definition.Name)#>Repository : I<#cs Write(Definition.Name)#>Repository
{
	private readonly IMongoCollection<<#cs Write(Definition.Name)#>Model> _collection;
	private readonly MongoDbContext _context;

	public MongoDb<#cs Write(Definition.Name)#>Repository(MongoDbContext context)
	{
		_context = context;
		_collection = _context.Database.GetCollection<<#cs Write(Definition.Name)#>Model>(nameof(<#cs Write(Definition.Name)#>Model)[0..^5]);
	}

	public async IAsyncEnumerable<<#cs Write(Definition.Name)#>Model> GetAll()
	{
		var items = await _collection.FindAsync(FilterDefinition<<#cs Write(Definition.Name)#>Model>.Empty).ConfigureAwait(false);

		while (await items.MoveNextAsync().ConfigureAwait(false))
		{
			foreach (var item in items.Current)
			{
				yield return item;
			}
		}
	}

	public IQueryable<<#cs Write(Definition.Name)#>Model> GetAllQueryable()
	{
		return _collection.AsQueryable();
	}

	public async Task<<#cs Write(Definition.Name)#>Model?> Get(Guid id)
	{
		var result = await _collection.FindAsync(item => item.Id == id).ConfigureAwait(false);
		return await result.FirstOrDefaultAsync().ConfigureAwait(false);
	}

	public async Task Insert(<#cs Write(Definition.Name)#>Model model)
	{
		await _collection.InsertOneAsync(model).ConfigureAwait(false);
	}

	public async Task Update(<#cs Write(Definition.Name)#>Model model)
	{
		await _collection.FindOneAndReplaceAsync(item => item.Id == model.Id, model).ConfigureAwait(false);
	}

	public async Task Delete(Guid id)
	{
		await _collection.DeleteOneAsync(item => item.Id == id).ConfigureAwait(false);
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>