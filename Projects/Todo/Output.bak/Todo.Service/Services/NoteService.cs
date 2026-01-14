using Cosei.Service.Base;
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

public class NoteService
{
	private readonly INoteRepository _noteRepository;
	private readonly IPublisher _publisher;
	private readonly IMap _map;

	public NoteService(
		INoteRepository noteRepository,
		IPublisher publisher,
		IMap map)
	{
		_noteRepository = noteRepository;
		_publisher = publisher;
		_map = map;
	}

	public async Task CreateNote(CreateNote command, Guid userId)
	{
		var item = _map.From(command).To<NoteModel>();

		await _noteRepository.Insert(item).ConfigureAwait(false);

		var message = _map.From(item).To<NoteCreated>();
		await _publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task UpdateNote(UpdateNote command, Guid userId)
	{
		var item = await _noteRepository.Get(command.Id).ConfigureAwait(false);

		if (item == null)
		{
			throw new InvalidOperationException("Note not found!");
		}

		_map.From(command).To(item);

		await _noteRepository.Update(item).ConfigureAwait(false);

		var message = _map.From(item).To<NoteUpdated>();
		await _publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task DeleteNote(Guid id, Guid userId)
	{
		await _noteRepository.Delete(id).ConfigureAwait(false);

		var message = new NoteDeleted(id);
		await _publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task<Note?> GetItemAsync(Guid id, Guid userId)
	{
		var item = await _noteRepository.Get(id).ConfigureAwait(false);
		return _map.FromNullable(item)?.To<Note>();
	}

	public IAsyncEnumerable<NoteListItem> GetItemsForUserAsync(NoteFilter filter, Guid userId)
	{
		var enumerable = _noteRepository.GetAllQueryable();

		if (!string.IsNullOrEmpty(filter.Title))
			enumerable = enumerable.Where(e => e.Title.Contains(filter.Title));
		if (!string.IsNullOrEmpty(filter.Content))
			enumerable = enumerable.Where(e => e.Content.Contains(filter.Content));
		if (filter.Color != null)
			enumerable = enumerable.Where(e => e.Color == filter.Color);

		return _map.From(enumerable).To<NoteListItem>().ToAsyncEnumerable();
	}

	public IAsyncEnumerable<NoteListItem> GetItemsForUserAsync(Guid userId)
	{
		var enumerable = _noteRepository.GetAllQueryable();
		return _map.From(enumerable).To<NoteListItem>().ToAsyncEnumerable();
	}
}