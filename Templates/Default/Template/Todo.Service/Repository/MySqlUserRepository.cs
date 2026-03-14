using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Service.Model;

namespace <#cs Write(Project.Namespace)#>.Service.Repository;

public class MySqlUserRepository(MySqlDbContext context) : IUserRepository
{
	public IAsyncEnumerable<UserModel> GetAll()
	{
		return context.Users
			.AsNoTracking()
			.AsAsyncEnumerable();
	}

	public IQueryable<UserModel> GetAllQueryable()
	{
		return context
			.Users
			.AsNoTracking()
			.AsQueryable();
	}

	public async Task<UserModel?> Get(Guid id)
	{
		var e = await context.Users.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

		if (e == null)
		{
			return null;
		}

		return e;
	}

	public async Task<UserModel?> Get(string username)
	{
		var e = await context.Users.FirstOrDefaultAsync(i => i.Username == username && i.DeletedAt == null).ConfigureAwait(false);

		if (e == null)
		{
			return null;
		}

		return e;
	}

	public async Task Insert(UserModel model)
	{
		await context.Users.AddAsync(model).ConfigureAwait(false);
	}

	public Task Update(UserModel model)
	{
		context.Users.Update(model);
		return Task.CompletedTask;
	}

	public async Task Delete(Guid id)
	{
		var model = await context.Users.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

		if (model != null)
		{
			context.Users.Remove(model);
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>