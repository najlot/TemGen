using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Service.Model;

namespace <#cs Write(Project.Namespace)#>.Service.Repository;

public class MySql<#cs Write(Definition.Name)#>Repository : I<#cs Write(Definition.Name)#>Repository
{
	private readonly MySqlDbContext _context;

	public MySql<#cs Write(Definition.Name)#>Repository(MySqlDbContext context)
	{
		_context = context;
	}

	public IAsyncEnumerable<<#cs Write(Definition.Name)#>Model> GetAll()
	{
		return _context
			.<#cs Write(Definition.Name)#>s
			.AsNoTracking()
			.AsAsyncEnumerable();
	}

	public IQueryable<<#cs Write(Definition.Name)#>Model> GetAllQueryable()
	{
		return _context
			.<#cs Write(Definition.Name)#>s
			.AsNoTracking()
			.AsQueryable();
	}

	public async Task<<#cs Write(Definition.Name)#>Model?> Get(Guid id)
	{
		var e = await _context.<#cs Write(Definition.Name)#>s.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

		if (e == null)
		{
			return null;
		}

<#cs
foreach(var entry in Entries)
{
	if (entry.IsReference)
	{
		// Owned types are loaded automatically
		// WriteLine($"		await _context.Entry(e).Reference(r => r.{entry.Field}).LoadAsync().ConfigureAwait(false);");
	}
}

#>
		return e;
	}

	public async Task Insert(<#cs Write(Definition.Name)#>Model model)
	{
<#cs
foreach(var entry in Entries.Where(e => e.IsArray))
{
	WriteLine("");
	WriteLine($"		foreach (var entry in model.{entry.Field})");
	WriteLine("		{");
	WriteLine("			entry.Id = 0;");
	WriteLine("		}");
	WriteLine("");
}

Result = Result.TrimEnd();
if(Entries.Where(e => e.IsArray).Any()) WriteLine("");
#>
		await _context.<#cs Write(Definition.Name)#>s.AddAsync(model).ConfigureAwait(false);

		await _context.SaveChangesAsync().ConfigureAwait(false);
	}

	public async Task Update(<#cs Write(Definition.Name)#>Model model)
	{
		_context.<#cs Write(Definition.Name)#>s.Update(model);

		await _context.SaveChangesAsync().ConfigureAwait(false);
	}

	public async Task Delete(Guid id)
	{
		var model = await _context.<#cs Write(Definition.Name)#>s.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

		if (model != null)
		{
			_context.<#cs Write(Definition.Name)#>s.Remove(model);
			await _context.SaveChangesAsync().ConfigureAwait(false);
		}
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>