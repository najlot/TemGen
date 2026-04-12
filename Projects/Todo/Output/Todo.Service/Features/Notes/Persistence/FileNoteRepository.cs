using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Todo.Service.Serialization;
using Todo.Service.Shared.Configuration;
using Todo.Service.Features.Notes;


namespace Todo.Service.Features.Notes.Persistence;

public class FileNoteRepository : INoteRepository
{
	private readonly string _storagePath;

	public FileNoteRepository(FileConfiguration configuration)
	{
		_storagePath = configuration.NotesPath;
		Directory.CreateDirectory(_storagePath);
	}

	public IQueryable<NoteModel> GetAllQueryable()
	{
		var items = new List<NoteModel>();

		foreach (var path in Directory.GetFiles(_storagePath))
		{
			var bytes = File.ReadAllBytes(path);
			var item = JsonSerializer.Deserialize<NoteModel>(bytes, ServiceJsonSerializer.Options);
			if (item is not null)
			{
				items.Add(item);
			}
		}

		return items.AsQueryable();
	}

	public async Task<NoteModel?> Get(Guid id)
	{
		var path = Path.Combine(_storagePath, id.ToString());

		if (!File.Exists(path))
		{
			return null;
		}

		var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
		var item = JsonSerializer.Deserialize<NoteModel>(bytes, ServiceJsonSerializer.Options);

		return item;
	}

	public async Task Insert(NoteModel model)
	{
		await Update(model).ConfigureAwait(false);
	}

	public async Task Update(NoteModel model)
	{
		var path = Path.Combine(_storagePath, model.Id.ToString());
		var bytes = JsonSerializer.SerializeToUtf8Bytes(model, ServiceJsonSerializer.Options);
		await File.WriteAllBytesAsync(path, bytes).ConfigureAwait(false);
	}

	public Task Delete(Guid id)
	{
		var path = Path.Combine(_storagePath, id.ToString());
		File.Delete(path);
		return Task.CompletedTask;
	}
}