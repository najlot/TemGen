using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Najlot.Map;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Service.Model;
using <# Project.Namespace#>.Service.Repository;
using <# Project.Namespace#>.Contracts.Commands;
using <# Project.Namespace#>.Contracts.Events;
using <# Project.Namespace#>.Contracts.ListItems;
using <# Project.Namespace#>.Contracts.Filters;

namespace <# Project.Namespace#>.Service.Services;

public class <# Definition.Name#>Service(
	I<# Definition.Name#>Repository <# Definition.NameLow#>Repository,
<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))
#>	I<# definition.Name#>Repository <# definition.NameLow#>Repository,
<#end#>	IPublisher publisher,
	IMap map)
{
	public async Task Create<# Definition.Name#>(Create<# Definition.Name#> command, Guid userId)
	{
		var item = map.From(command).To<<# Definition.Name#>Model>();

		await <# Definition.NameLow#>Repository.Insert(item).ConfigureAwait(false);

		var message = map.From(item).To<<# Definition.Name#>Created>();
		await publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task Update<# Definition.Name#>(Update<# Definition.Name#> command, Guid userId)
	{
		var item = await <# Definition.NameLow#>Repository.Get(command.Id).ConfigureAwait(false);

		if (item == null)
		{
			throw new InvalidOperationException("<# Definition.Name#> not found!");
		}

		map.From(command).To(item);

		await <# Definition.NameLow#>Repository.Update(item).ConfigureAwait(false);

		var message = map.From(item).To<<# Definition.Name#>Updated>();
		await publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task Delete<# Definition.Name#>(Guid id, Guid userId)
	{
		var item = await <# Definition.NameLow#>Repository.Get(id).ConfigureAwait(false);

		if (item == null)
		{
			throw new InvalidOperationException("<# Definition.Name#> not found!");
		}

		if (item.DeletedAt == null)
		{
			item.DeletedAt = DateTime.UtcNow;
			await <# Definition.NameLow#>Repository.Update(item).ConfigureAwait(false);
		}
		else
		{
			await <# Definition.NameLow#>Repository.Delete(id).ConfigureAwait(false);
		}

		var message = new <# Definition.Name#>Deleted(id);
		await publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task<<# Definition.Name#>?> GetItemAsync(Guid id, Guid userId)
	{
		var item = await <# Definition.NameLow#>Repository.Get(id).ConfigureAwait(false);
		return map.FromNullable(item)?.To<<# Definition.Name#>>();
	}

	public IAsyncEnumerable<<# Definition.Name#>ListItem> GetItemsForUserAsync(<# Definition.Name#>Filter filter, Guid userId)
	{
		var query = <# Definition.NameLow#>Repository.GetAllQueryable();

		query = query.Where(e => e.DeletedAt == null);

<#for entry in Entries
#><#if entry.IsReference
#>		if (filter.<# entry.Field#>Id != null)
			query = query.Where(e => e.<# entry.Field#>Id == filter.<# entry.Field#>Id);
<#elseif entry.EntryType == "long"
		|| entry.EntryType == "short"
		|| entry.EntryType == "int"
		|| entry.EntryType == "ulong"
		|| entry.EntryType == "ushort"
		|| entry.EntryType == "uint"
		|| entry.EntryType == "DateTime"
        
#>		if (filter.<# entry.Field#>From != null)
			query = query.Where(e => e.<# entry.Field#> >= filter.<# entry.Field#>From);

		if (filter.<# entry.Field#>To != null)
			query = query.Where(e => e.<# entry.Field#> <= filter.<# entry.Field#>To);
<#elseif entry.EntryType.ToLower() == "string"
#>		if (!string.IsNullOrEmpty(filter.<# entry.Field#>))
			query = query.Where(e => e.<# entry.Field#>.Contains(filter.<# entry.Field#>));
<#elseif !(entry.IsArray || entry.IsOwnedType)
#>		if (filter.<# entry.Field#> != null)
			query = query.Where(e => e.<# entry.Field#> == filter.<# entry.Field#>);
<#end#><#end#>

		return map.From(query).To<<# Definition.Name#>ListItem>().ToAsyncEnumerable();
	}

	public IAsyncEnumerable<<# Definition.Name#>ListItem> GetItemsForUserAsync(Guid userId)
	{
		var query = <# Definition.NameLow#>Repository.GetAllQueryable();

		query = query.Where(e => e.DeletedAt == null);

		return map.From(query).To<<# Definition.Name#>ListItem>().ToAsyncEnumerable();
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>