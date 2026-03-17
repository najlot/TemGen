using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Model;

namespace <# Project.Namespace#>.Service.Repository;

public class MySql<# Definition.Name#>Repository(MySqlDbContext context) : I<# Definition.Name#>Repository
{
	public IAsyncEnumerable<<# Definition.Name#>Model> GetAll()
	{
		return context
			.<# Definition.Name#>s
			.AsNoTracking()
			.AsAsyncEnumerable();
	}

	public IQueryable<<# Definition.Name#>Model> GetAllQueryable()
	{
		return context
			.<# Definition.Name#>s
			.AsNoTracking()
			.AsQueryable();
	}

	public async Task<<# Definition.Name#>Model?> Get(Guid id)
	{
		var e = await context.<# Definition.Name#>s.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

		if (e == null)
		{
			return null;
		}

		return e;
	}

	public async Task Insert(<# Definition.Name#>Model model)
	{
<#for entry in Entries.Where(e => e.IsArray)
#>
		foreach (var entry in model.<# entry.Field#>)
		{
			entry.Id = 0;
		}

<#end#>
		await context.<# Definition.Name#>s.AddAsync(model).ConfigureAwait(false);
	}

	public Task Update(<# Definition.Name#>Model model)
	{
		context.<# Definition.Name#>s.Update(model);
		return Task.CompletedTask;
	}

	public async Task Delete(Guid id)
	{
		var model = await context.<# Definition.Name#>s.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

		if (model != null)
		{
			context.<# Definition.Name#>s.Remove(model);
		}
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>