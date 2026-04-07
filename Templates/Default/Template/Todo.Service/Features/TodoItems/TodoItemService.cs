using Najlot.Map;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Contracts.Commands;
using <# Project.Namespace#>.Contracts.Events;
using <# Project.Namespace#>.Contracts.ListItems;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Service.Features.Auth;
using <# Project.Namespace#>.Service.Features.History;
using <# Project.Namespace#>.Service.Shared.Realtime;
using <# Project.Namespace#>.Service.Shared.Results;

namespace <# Project.Namespace#>.Service.Features.<# Definition.Name#>s;

public class <# Definition.Name#>Service(
	I<# Definition.Name#>Repository <# Definition.NameLow#>Repository,
	HistoryService historyService,
	IPublisher publisher,
	IMap map,
	IPermissionQueryFilter permissionQueryFilter)
{
	public async Task<Result> Create<# Definition.Name#>(Create<# Definition.Name#> command)
	{
		var item = new <# Definition.Name#>Model();
		var snapshot = historyService.CreateSnapshot(item);
		map.From(command).To(item);

		await <# Definition.NameLow#>Repository.Insert(item).ConfigureAwait(false);
		await historyService.WriteChangesAsync(item.Id, snapshot).ConfigureAwait(false);

		var message = map.From(item).To<<# Definition.Name#>Created>();
		await publisher.PublishAsync(message).ConfigureAwait(false);
		return Result.Success();
	}

	public async Task<Result> Update<# Definition.Name#>(Update<# Definition.Name#> command)
	{
		var item = await <# Definition.NameLow#>Repository.Get(command.Id).ConfigureAwait(false);

		if (item == null)
		{
			return Result.NotFound("<# Definition.Name#> not found!");
		}

		var snapshot = historyService.CreateSnapshot(item);
		map.From(command).To(item);

		await <# Definition.NameLow#>Repository.Update(item).ConfigureAwait(false);
		await historyService.WriteChangesAsync(item.Id, snapshot).ConfigureAwait(false);

		var message = map.From(item).To<<# Definition.Name#>Updated>();
		await publisher.PublishAsync(message).ConfigureAwait(false);

		if (item.DeletedAt != null)
		{
			var trashItemUpdated = map.From(item).To<TrashItemUpdated>();
			await publisher.PublishAsync(trashItemUpdated).ConfigureAwait(false);
		}

		return Result.Success();
	}

	public async Task<Result> Delete<# Definition.Name#>(Guid id)
	{
		var item = await <# Definition.NameLow#>Repository.Get(id).ConfigureAwait(false);

		if (item == null)
		{
			return Result.NotFound("<# Definition.Name#> not found!");
		}

		if (item.DeletedAt == null)
		{
			var snapshot = historyService.CreateSnapshot(item);
			item.DeletedAt = DateTime.UtcNow;
			await <# Definition.NameLow#>Repository.Update(item).ConfigureAwait(false);
			await historyService.WriteChangesAsync(item.Id, snapshot).ConfigureAwait(false);

			var trashItemCreated = map.From(item).To<TrashItemCreated>();
			await publisher.PublishAsync(trashItemCreated).ConfigureAwait(false);

			var message = new <# Definition.Name#>Deleted(id);
			await publisher.PublishAsync(message).ConfigureAwait(false);
		}
		else
		{
			await historyService.DeleteHistoryEntriesAsync(item.Id).ConfigureAwait(false);
			await <# Definition.NameLow#>Repository.Delete(id).ConfigureAwait(false);
			var trashItemDeleted = new TrashItemDeleted(item.Id, ItemType.<# Definition.Name#>);
			await publisher.PublishAsync(trashItemDeleted).ConfigureAwait(false);
		}

		return Result.Success();
	}

	public async Task<Result<<# Definition.Name#>>> GetItemAsync(Guid id)
	{
		var item = await <# Definition.NameLow#>Repository.Get(id).ConfigureAwait(false);

		if (item == null)
		{
			return Result<<# Definition.Name#>>.NotFound("<# Definition.Name#> not found!");
		}

		return Result<<# Definition.Name#>>.Success(map.From(item).To<<# Definition.Name#>>());
	}

	public IAsyncEnumerable<<# Definition.Name#>ListItem> GetItemsForUserAsync(<# Definition.Name#>Filter filter)
	{
		var query = <# Definition.NameLow#>Repository.GetAllQueryable();

		query = query.Where(e => e.DeletedAt == null);
		query = permissionQueryFilter.ApplyReadFilter(query);

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

	public IAsyncEnumerable<<# Definition.Name#>ListItem> GetItemsForUserAsync()
	{
		var query = <# Definition.NameLow#>Repository.GetAllQueryable();

		query = query.Where(e => e.DeletedAt == null);
		query = permissionQueryFilter.ApplyReadFilter(query);

		return map.From(query).To<<# Definition.Name#>ListItem>().ToAsyncEnumerable();
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>