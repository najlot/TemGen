using Najlot.Map;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Contracts.<# Definition.Name#>s;
using <# Project.Namespace#>.Contracts.Trash;
using <# Project.Namespace#>.Contracts.Shared;
using <# Project.Namespace#>.Service.Features.Auth;
using <# Project.Namespace#>.Service.Features.History;
using <# Project.Namespace#>.Service.Features.Filters;
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
	private static readonly HashSet<string> FilterableProperties = new(StringComparer.Ordinal)
	{
<#for entry in Entries.Where(e => !(e.IsKey || e.IsArray || e.IsOwnedType))
#>		nameof(<# Definition.Name#>Model.<#cs Write(GetFieldPropertyName(entry.Field, entry.IsReference))#>),
<#end#>	};

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

	public IAsyncEnumerable<<# Definition.Name#>ListItem> GetItemsForUserAsync(EntityFilter filter)
	{
		if (filter.Conditions.Count == 0)
		{
			return GetItemsForUserAsync();
		}

		var query = <# Definition.NameLow#>Repository.GetAllQueryable();

		query = query.Where(e => e.DeletedAt == null);
		query = permissionQueryFilter.ApplyReadFilter(query);

		foreach (var condition in filter.Conditions)
		{
			if (!FilterableProperties.Contains(condition.Field))
			{
				continue;
			}

			query = query.ApplyFilter(condition.Field, condition);
		}

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