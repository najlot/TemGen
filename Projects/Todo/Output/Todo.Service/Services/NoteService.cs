using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
	IMap map)
{
	public async Task CreateNote(CreateNote command, Guid userId)
	{
		var item = map.From(command).To<NoteModel>();

		await noteRepository.Insert(item).ConfigureAwait(false);

		var message = map.From(item).To<NoteCreated>();
		await publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task UpdateNote(UpdateNote command, Guid userId)
	{
		var item = await noteRepository.Get(command.Id).ConfigureAwait(false);

		if (item == null)
		{
			throw new InvalidOperationException("Note not found!");
		}

		map.From(command).To(item);

		await noteRepository.Update(item).ConfigureAwait(false);

		var message = map.From(item).To<NoteUpdated>();
		await publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task DeleteNote(Guid id, Guid userId)
	{
		var item = await noteRepository.Get(id).ConfigureAwait(false);

		if (item == null)
		{
			throw new InvalidOperationException("Note not found!");
		}

		if (item.DeletedAt == null)
		{
			item.DeletedAt = DateTime.UtcNow;
			await noteRepository.Update(item).ConfigureAwait(false);
		}
		else
		{
			await noteRepository.Delete(id).ConfigureAwait(false);
		}

		var message = new NoteDeleted(id);
		await publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task<Note?> GetItemAsync(Guid id, Guid userId)
	{
		var item = await noteRepository.Get(id).ConfigureAwait(false);
		return map.FromNullable(item)?.To<Note>();
	}

	public IAsyncEnumerable<NoteListItem> GetItemsForUserAsync(NoteFilter filter, Guid userId)
	{
		var query = noteRepository.GetAllQueryable();

		query = query.Where(e => e.DeletedAt == null);

		if (!string.IsNullOrEmpty(filter.Title))
			query = query.Where(e => e.Title.Contains(filter.Title));
		if (!string.IsNullOrEmpty(filter.Content))
			query = query.Where(e => e.Content.Contains(filter.Content));
		if (filter.Color != null)
			query = query.Where(e => e.Color == filter.Color);

		return map.From(query).To<NoteListItem>().ToAsyncEnumerable();
	}

	public IAsyncEnumerable<NoteListItem> GetItemsForUserAsync(Guid userId)
	{
		var query = noteRepository.GetAllQueryable();

		query = query.Where(e => e.DeletedAt == null);

		return map.From(query).To<NoteListItem>().ToAsyncEnumerable();
	}
}