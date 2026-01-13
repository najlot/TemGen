using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Model;

namespace Todo.Service.Repository;

public class MySqlUserRepository : IUserRepository
{
	private readonly MySqlDbContext _context;

	public MySqlUserRepository(MySqlDbContext context)
	{
		_context = context;
	}

	public IAsyncEnumerable<UserModel> GetAll()
	{
		return _context.Users
			.AsNoTracking()
			.AsAsyncEnumerable();
	}

	public async Task<UserModel?> Get(Guid id)
	{
		var e = await _context.Users.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

		if (e == null)
		{
			return null;
		}

		return e;
	}

	public async Task<UserModel?> Get(string username)
	{
		var e = await _context.Users.FirstOrDefaultAsync(i => i.Username == username && i.IsActive).ConfigureAwait(false);

		if (e == null)
		{
			return null;
		}

		return e;
	}

	public async Task Insert(UserModel model)
	{
		await _context.Users.AddAsync(model).ConfigureAwait(false);
		await _context.SaveChangesAsync().ConfigureAwait(false);
	}

	public async Task Update(UserModel model)
	{
		_context.Users.Update(model);
		await _context.SaveChangesAsync().ConfigureAwait(false);
	}

	public async Task Delete(Guid id)
	{
		var model = await _context.Users.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

		if (model != null)
		{
			_context.Users.Remove(model);
			await _context.SaveChangesAsync().ConfigureAwait(false);
		}
	}
}