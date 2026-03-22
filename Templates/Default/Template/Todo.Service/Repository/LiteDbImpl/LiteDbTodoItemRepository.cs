using LiteDB;
using System;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Model;

namespace <# Project.Namespace#>.Service.Repository.LiteDbImpl;

public class LiteDb<# Definition.Name#>Repository : I<# Definition.Name#>Repository
{
	private readonly ILiteCollection<<# Definition.Name#>Model> _collection;

	public LiteDb<# Definition.Name#>Repository(LiteDbContext context)
	{
		_collection = context.GetCollection<<# Definition.Name#>Model>("<# Definition.Name#>s");
	}

	public IQueryable<<# Definition.Name#>Model> GetAllQueryable()
	{
		return _collection.FindAll().AsQueryable();
	}

	public Task<<# Definition.Name#>Model?> Get(Guid id)
	{
		var model = _collection.FindById(id);
		return Task.FromResult<<# Definition.Name#>Model?>(model);
	}

	public Task Insert(<# Definition.Name#>Model model)
	{
		_collection.Insert(model);
		return Task.CompletedTask;
	}

	public Task Update(<# Definition.Name#>Model model)
	{
		_collection.Upsert(model);
		return Task.CompletedTask;
	}

	public Task Delete(Guid id)
	{
		_collection.Delete(id);
		return Task.CompletedTask;
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>