using Cosei.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Najlot.Map;
using <#cs Write(Project.Namespace)#>.Contracts;
using <#cs Write(Project.Namespace)#>.Service.Model;
using <#cs Write(Project.Namespace)#>.Service.Repository;
using <#cs Write(Project.Namespace)#>.Contracts.Commands;
using <#cs Write(Project.Namespace)#>.Contracts.Events;
using <#cs Write(Project.Namespace)#>.Contracts.ListItems;
using <#cs Write(Project.Namespace)#>.Contracts.Filters;

namespace <#cs Write(Project.Namespace)#>.Service.Services;

public class <#cs Write(Definition.Name)#>Service
{
	private readonly I<#cs Write(Definition.Name)#>Repository _<#cs Write(Definition.NameLow)#>Repository;
<#cs
foreach (var definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n)))
{
WriteLine($"	private readonly I{definition.Name}Repository _{definition.NameLow}Repository;");
}
#>	private readonly IPublisher _publisher;
	private readonly IMap _map;

	public <#cs Write(Definition.Name)#>Service(
		I<#cs Write(Definition.Name)#>Repository <#cs Write(Definition.NameLow)#>Repository,
<#cs
foreach (var definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n)))
{
WriteLine($"			I{definition.Name}Repository {definition.NameLow}Repository,");
}
#>		IPublisher publisher,
		IMap map)
	{
		_<#cs Write(Definition.NameLow)#>Repository = <#cs Write(Definition.NameLow)#>Repository;
<#cs
foreach (var definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n)))
{
WriteLine($"			_{definition.NameLow}Repository = {definition.NameLow}Repository;");
}
#>		_publisher = publisher;
		_map = map;
	}

	public async Task Create<#cs Write(Definition.Name)#>(Create<#cs Write(Definition.Name)#> command, Guid userId)
	{
		var item = _map.From(command).To<<#cs Write(Definition.Name)#>Model>();

		await _<#cs Write(Definition.NameLow)#>Repository.Insert(item).ConfigureAwait(false);

		var message = _map.From(item).To<<#cs Write(Definition.Name)#>Created>();
		await _publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task Update<#cs Write(Definition.Name)#>(Update<#cs Write(Definition.Name)#> command, Guid userId)
	{
		var item = await _<#cs Write(Definition.NameLow)#>Repository.Get(command.Id).ConfigureAwait(false);

		if (item == null)
		{
			throw new InvalidOperationException("<#cs Write(Definition.Name)#> not found!");
		}

		_map.From(command).To(item);

		await _<#cs Write(Definition.NameLow)#>Repository.Update(item).ConfigureAwait(false);

		var message = _map.From(item).To<<#cs Write(Definition.Name)#>Updated>();
		await _publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task Delete<#cs Write(Definition.Name)#>(Guid id, Guid userId)
	{
		await _<#cs Write(Definition.NameLow)#>Repository.Delete(id).ConfigureAwait(false);

		var message = new <#cs Write(Definition.Name)#>Deleted(id);
		await _publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task<<#cs Write(Definition.Name)#>?> GetItemAsync(Guid id, Guid userId)
	{
		var item = await _<#cs Write(Definition.NameLow)#>Repository.Get(id).ConfigureAwait(false);
		return _map.FromNullable(item)?.To<<#cs Write(Definition.Name)#>>();
	}

	public IAsyncEnumerable<<#cs Write(Definition.Name)#>ListItem> GetItemsForUserAsync(<#cs Write(Definition.Name)#>Filter filter, Guid userId)
	{
		var enumerable = _<#cs Write(Definition.NameLow)#>Repository.GetAllQueryable();

<#cs
foreach(var entry in Entries)
{
    if (entry.IsReference)
    {
        WriteLine($"		if (filter.{entry.Field}Id != null)");
		WriteLine($"			enumerable = enumerable.Where(e => e.{entry.Field}Id == filter.{entry.Field}Id);");
    }
    else if (entry.EntryType == "long"
        || entry.EntryType == "short"
        || entry.EntryType == "int"
        || entry.EntryType == "ulong"
        || entry.EntryType == "ushort"
        || entry.EntryType == "uint"
        || entry.EntryType == "DateTime"
        )
    {
        WriteLine($"		if (filter.{entry.Field}From != null)");
		WriteLine($"			enumerable = enumerable.Where(e => e.{entry.Field} >= filter.{entry.Field}From);");

		WriteLine($"		if (filter.{entry.Field}To != null)");
		WriteLine($"			enumerable = enumerable.Where(e => e.{entry.Field} <= filter.{entry.Field}To);");
    }
	else if (entry.EntryType.ToLower() == "string")
    {
        WriteLine($"		if (!string.IsNullOrEmpty(filter.{entry.Field}))");
		WriteLine($"			enumerable = enumerable.Where(e => e.{entry.Field}.Contains(filter.{entry.Field}));");
    }
    else if (!(entry.IsArray || entry.IsOwnedType))
    {
        WriteLine($"		if (filter.{entry.Field} != null)");
		WriteLine($"			enumerable = enumerable.Where(e => e.{entry.Field} == filter.{entry.Field});");
    }
}

Result = Result.TrimEnd();
#>

		return _map.From(enumerable).To<<#cs Write(Definition.Name)#>ListItem>().ToAsyncEnumerable();
	}

	public IAsyncEnumerable<<#cs Write(Definition.Name)#>ListItem> GetItemsForUserAsync(Guid userId)
	{
		var enumerable = _<#cs Write(Definition.NameLow)#>Repository.GetAllQueryable();
		return _map.From(enumerable).To<<#cs Write(Definition.Name)#>ListItem>().ToAsyncEnumerable();
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>