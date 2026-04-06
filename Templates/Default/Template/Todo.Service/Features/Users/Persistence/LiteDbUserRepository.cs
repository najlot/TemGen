using LiteDB;
using System;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Features.Users;
using <# Project.Namespace#>.Service.Infrastructure.Persistence.LiteDb;

namespace <# Project.Namespace#>.Service.Features.Users.Persistence;

public class LiteDbUserRepository : IUserRepository
{
	private readonly ILiteCollection<UserModel> _collection;

	public LiteDbUserRepository(LiteDbContext context)
	{
		_collection = context.GetCollection<UserModel>("Users");
	}

	public IQueryable<UserModel> GetAllQueryable()
	{
		return _collection.FindAll().AsQueryable();
	}

	public Task<UserModel?> Get(Guid id)
	{
		var user = _collection.FindById(id);
		return Task.FromResult<UserModel?>(user);
	}

	public Task<UserModel?> Get(string username)
	{
		var user = _collection.FindOne(item => item.Username == username && item.DeletedAt == null);
		return Task.FromResult<UserModel?>(user);
	}

	public Task Insert(UserModel model)
	{
		_collection.Insert(model);
		return Task.CompletedTask;
	}

	public Task Update(UserModel model)
	{
		_collection.Upsert(model);
		return Task.CompletedTask;
	}

	public Task Delete(Guid id)
	{
		_collection.Delete(id);
		return Task.CompletedTask;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>