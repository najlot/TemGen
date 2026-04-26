using Najlot.Map;
using Todo.Contracts.Filters;
using Todo.Contracts.Notes;
using Todo.Contracts.Trash;
using Todo.Contracts.Shared;
using Todo.Service.Features.Auth;
using Todo.Service.Features.History;
using Todo.Service.Features.Filters;
using Todo.Service.Shared.Realtime;
using Todo.Service.Shared.Results;

namespace Todo.Service.Features.Notes;

public class NoteService(
	INoteRepository noteRepository,
	HistoryService historyService,
	IPublisher publisher,
	IMap map,
	IPermissionQueryFilter permissionQueryFilter)
{
	private static readonly HashSet<string> FilterableProperties = new(StringComparer.Ordinal)
	{
		nameof(NoteModel.Title),
		nameof(NoteModel.Content),
		nameof(NoteModel.Color),
	};

	public async Task<Result> CreateNote(CreateNote command)
	{
		var item = new NoteModel();
		var snapshot = historyService.CreateSnapshot(item);
		map.From(command).To(item);

		await noteRepository.Insert(item).ConfigureAwait(false);
		await historyService.WriteChangesAsync(item.Id, snapshot).ConfigureAwait(false);

		var message = map.From(item).To<NoteCreated>();
		await publisher.PublishAsync(message).ConfigureAwait(false);
		return Result.Success();
	}

	public async Task<Result> UpdateNote(UpdateNote command)
	{
		var item = await noteRepository.Get(command.Id).ConfigureAwait(false);

		if (item == null)
		{
			return Result.NotFound("Note not found!");
		}

		var snapshot = historyService.CreateSnapshot(item);
		map.From(command).To(item);

		await noteRepository.Update(item).ConfigureAwait(false);
		await historyService.WriteChangesAsync(item.Id, snapshot).ConfigureAwait(false);

		var message = map.From(item).To<NoteUpdated>();
		await publisher.PublishAsync(message).ConfigureAwait(false);

		if (item.DeletedAt != null)
		{
			var trashItemUpdated = map.From(item).To<TrashItemUpdated>();
			await publisher.PublishAsync(trashItemUpdated).ConfigureAwait(false);
		}

		return Result.Success();
	}

	public async Task<Result> DeleteNote(Guid id)
	{
		var item = await noteRepository.Get(id).ConfigureAwait(false);

		if (item == null)
		{
			return Result.NotFound("Note not found!");
		}

		if (item.DeletedAt == null)
		{
			var snapshot = historyService.CreateSnapshot(item);
			item.DeletedAt = DateTime.UtcNow;
			await noteRepository.Update(item).ConfigureAwait(false);
			await historyService.WriteChangesAsync(item.Id, snapshot).ConfigureAwait(false);

			var trashItemCreated = map.From(item).To<TrashItemCreated>();
			await publisher.PublishAsync(trashItemCreated).ConfigureAwait(false);

			var message = new NoteDeleted(id);
			await publisher.PublishAsync(message).ConfigureAwait(false);
		}
		else
		{
			await historyService.DeleteHistoryEntriesAsync(item.Id).ConfigureAwait(false);
			await noteRepository.Delete(id).ConfigureAwait(false);
			var trashItemDeleted = new TrashItemDeleted(item.Id, ItemType.Note);
			await publisher.PublishAsync(trashItemDeleted).ConfigureAwait(false);
		}

		return Result.Success();
	}

	public async Task<Result<Note>> GetItemAsync(Guid id)
	{
		var item = await noteRepository.Get(id).ConfigureAwait(false);

		if (item == null)
		{
			return Result<Note>.NotFound("Note not found!");
		}

		return Result<Note>.Success(map.From(item).To<Note>());
	}

	public IAsyncEnumerable<NoteListItem> GetItemsForUserAsync(EntityFilter filter)
	{
		if (filter.Conditions.Count == 0)
		{
			return GetItemsForUserAsync();
		}

		var query = noteRepository.GetAllQueryable();

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

		return map.From(query).To<NoteListItem>().ToAsyncEnumerable();
	}

	public IAsyncEnumerable<NoteListItem> GetItemsForUserAsync()
	{
		var query = noteRepository.GetAllQueryable();

		query = query.Where(e => e.DeletedAt == null);
		query = permissionQueryFilter.ApplyReadFilter(query);

		return map.From(query).To<NoteListItem>().ToAsyncEnumerable();
	}
}