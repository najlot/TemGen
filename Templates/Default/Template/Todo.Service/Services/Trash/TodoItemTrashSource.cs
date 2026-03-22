<#if Definition.Name == "TodoItem"
#>using LiteDB;
using Najlot.Map;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Contracts.Events;
using <# Project.Namespace#>.Service.Model;
using <# Project.Namespace#>.Service.Repository;
<#else#>using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Contracts.Events;
using <# Project.Namespace#>.Service.Repository;
using Najlot.Map;
using <# Project.Namespace#>.Service.Model;
<#end#>
namespace <# Project.Namespace#>.Service.Services.Trash;

public class <# Definition.Name#>TrashSource(
	I<# Definition.Name#>Repository <# Definition.NameLow#>Repository,
	IPublisher publisher,
	IMap map,
	IPermissionQueryFilter permissionQueryFilter) : ITrashSource
{
	public ItemType Type => ItemType.<# Definition.Name#>;

	public IAsyncEnumerable<TrashItem> GetItemsAsync()
	{
		var query = permissionQueryFilter
			.ApplyReadFilter(<# Definition.NameLow#>Repository.GetAllQueryable())
			.Where(item => item.DeletedAt != null)
			.OrderByDescending(item => item.DeletedAt);

		return map.From<<# Definition.Name#>Model>(query).To<TrashItem>().ToAsyncEnumerable();
	}

	public async Task<Result> RestoreAsync(Guid id)
	{
		var item = await <# Definition.NameLow#>Repository.Get(id).ConfigureAwait(false);

		if (item == null || item.DeletedAt == null)
		{
			return Result.NotFound("Trash item not found!");
		}

		item.DeletedAt = null;
		await <# Definition.NameLow#>Repository.Update(item).ConfigureAwait(false);

		await publisher.PublishAsync(new TrashItemDeleted(item.Id, ItemType.<# Definition.Name#>)).ConfigureAwait(false);
		await publisher.PublishAsync(map.From(item).To<<# Definition.Name#>Created>()).ConfigureAwait(false);
		return Result.Success();
	}

	public async Task<Result> DeleteAsync(Guid id)
	{
		var item = await <# Definition.NameLow#>Repository.Get(id).ConfigureAwait(false);

		if (item == null || item.DeletedAt == null)
		{
			return Result.NotFound("Trash item not found!");
		}

		await <# Definition.NameLow#>Repository.Delete(id).ConfigureAwait(false);
		await publisher.PublishAsync(new TrashItemDeleted(item.Id, ItemType.<# Definition.Name#>)).ConfigureAwait(false);
		return Result.Success();
	}

	public async Task DeleteAllAsync()
	{
		var items = permissionQueryFilter
			.ApplyReadFilter(<# Definition.NameLow#>Repository.GetAllQueryable())
			.Where(item => item.DeletedAt != null)
			.ToList();

		foreach (var item in items)
		{
			await <# Definition.NameLow#>Repository.Delete(item.Id).ConfigureAwait(false);
			await publisher.PublishAsync(new TrashItemDeleted(item.Id, ItemType.<# Definition.Name#>)).ConfigureAwait(false);
		}
	}
}
<#cs SetOutputPath(Definition.IsOwnedType
	|| Definition.IsEnumeration
	|| Definition.IsArray
	|| Definition.NameLow == "user")#>