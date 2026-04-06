using Todo.Contracts;
using Todo.Contracts.Events;
using Todo.Service.Features.Auth;
using Todo.Service.Features.History;
using Todo.Service.Features.Notes;
using Todo.Service.Shared.Realtime;
using Todo.Service.Shared.Results;

using Najlot.Map;


namespace Todo.Service.Features.Trash;

public class NoteTrashSource(
	INoteRepository noteRepository,
	HistoryService historyService,
	IPublisher publisher,
	IMap map,
	IPermissionQueryFilter permissionQueryFilter) : ITrashSource
{
	public ItemType Type => ItemType.Note;

	public IAsyncEnumerable<TrashItem> GetItemsAsync()
	{
		var query = permissionQueryFilter
			.ApplyReadFilter(noteRepository.GetAllQueryable())
			.Where(item => item.DeletedAt != null)
			.OrderByDescending(item => item.DeletedAt);

		return map.From<NoteModel>(query).To<TrashItem>().ToAsyncEnumerable();
	}

	public async Task<Result> RestoreAsync(Guid id)
	{
		var item = await noteRepository.Get(id).ConfigureAwait(false);

		if (item == null || item.DeletedAt == null)
		{
			return Result.NotFound("Trash item not found!");
		}

		var snapshot = historyService.CreateSnapshot(item);
		item.DeletedAt = null;
		await noteRepository.Update(item).ConfigureAwait(false);
		await historyService.WriteChangesAsync(item.Id, snapshot).ConfigureAwait(false);

		await publisher.PublishAsync(new TrashItemDeleted(item.Id, ItemType.Note)).ConfigureAwait(false);
		await publisher.PublishAsync(map.From(item).To<NoteCreated>()).ConfigureAwait(false);
		return Result.Success();
	}

	public async Task<Result> DeleteAsync(Guid id)
	{
		var item = await noteRepository.Get(id).ConfigureAwait(false);

		if (item == null || item.DeletedAt == null)
		{
			return Result.NotFound("Trash item not found!");
		}

		await historyService.DeleteHistoryEntriesAsync(item.Id).ConfigureAwait(false);
		await noteRepository.Delete(id).ConfigureAwait(false);
		await publisher.PublishAsync(new TrashItemDeleted(item.Id, ItemType.Note)).ConfigureAwait(false);
		return Result.Success();
	}

	public async Task DeleteAllAsync()
	{
		var items = permissionQueryFilter
			.ApplyReadFilter(noteRepository.GetAllQueryable())
			.Where(item => item.DeletedAt != null)
			.ToList();

		foreach (var item in items)
		{
			await historyService.DeleteHistoryEntriesAsync(item.Id).ConfigureAwait(false);
			await noteRepository.Delete(item.Id).ConfigureAwait(false);
			await publisher.PublishAsync(new TrashItemDeleted(item.Id, ItemType.Note)).ConfigureAwait(false);
		}
	}
}
