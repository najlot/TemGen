using Najlot.Map;
using Todo.Contracts;
using Todo.Service.Model;
using Todo.Service.Repository;
using Todo.Contracts.Commands;
using Todo.Contracts.Events;
using Todo.Contracts.ListItems;
using Todo.Contracts.Filters;

namespace Todo.Service.Services;

public class NoteService(
	INoteRepository noteRepository,
	IPublisher publisher,
	IMap map,
	IPermissionQueryFilter permissionQueryFilter)
{
	public async Task<Result> CreateNote(CreateNote command)
	{
		var item = map.From(command).To<NoteModel>();

		await noteRepository.Insert(item).ConfigureAwait(false);

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

		map.From(command).To(item);

		await noteRepository.Update(item).ConfigureAwait(false);

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
			item.DeletedAt = DateTime.UtcNow;
			await noteRepository.Update(item).ConfigureAwait(false);

			var trashItemCreated = map.From(item).To<TrashItemCreated>();
			await publisher.PublishAsync(trashItemCreated).ConfigureAwait(false);

			var message = new NoteDeleted(id);
			await publisher.PublishAsync(message).ConfigureAwait(false);
		}
		else
		{
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

	public IAsyncEnumerable<NoteListItem> GetItemsForUserAsync(NoteFilter filter)
	{
		var query = noteRepository.GetAllQueryable();

		query = query.Where(e => e.DeletedAt == null);
		query = permissionQueryFilter.ApplyReadFilter(query);

		if (!string.IsNullOrEmpty(filter.Title))
			query = query.Where(e => e.Title.Contains(filter.Title));
		if (!string.IsNullOrEmpty(filter.Content))
			query = query.Where(e => e.Content.Contains(filter.Content));
		if (filter.Color != null)
			query = query.Where(e => e.Color == filter.Color);

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