using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Najlot.Map;
using Todo.Client.Data.Models;
using Todo.Client.Data.Repositories;
using Todo.ClientBase.ProfileHandler;
using Todo.Contracts;
using Todo.Contracts.Events;
using Todo.Contracts.Filters;

namespace Todo.ClientBase.Services.Implementation;

public sealed class LocalNoteStore : INoteRepository
{
	private readonly string _dataPath;
	private readonly ILocalSubscriber _subscriber;
	private readonly IMap _map;
	private List<NoteModel> _items = null;

	public LocalNoteStore(string folderName, ILocalSubscriber localSubscriber, IMap map)
	{
		_map = map;

		var appdataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Todo");
		appdataDir = Path.Combine(appdataDir, folderName);
		Directory.CreateDirectory(appdataDir);

		_dataPath = Path.Combine(appdataDir, "Notes.json");
		_items = GetItems();
		_subscriber = localSubscriber;
	}

	private List<NoteModel> GetItems()
	{
		List<NoteModel> items;
		if (File.Exists(_dataPath))
		{
			var data = File.ReadAllText(_dataPath);
			items = JsonSerializer.Deserialize<List<NoteModel>>(data, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
		}
		else
		{
			items = new List<NoteModel>();
		}

		return items;
	}

	public async Task AddItemAsync(NoteModel item)
	{
		_items.Insert(0, item);

		SaveItems();

		await _subscriber.SendAsync(_map.From(item).To<NoteCreated>());
	}

	private void SaveItems()
	{
		var text = JsonSerializer.Serialize(_items);
		File.WriteAllText(_dataPath, text);
	}

	public async Task UpdateItemAsync(NoteModel item)
	{
		int index = 0;
		var oldItem = _items.FirstOrDefault(i => i.Id == item.Id);

		if (oldItem != null)
		{
			index = _items.IndexOf(oldItem);

			if (index != -1)
			{
				_items.RemoveAt(index);
			}
			else
			{
				index = 0;
			}
		}

		_items.Insert(index, item);

		SaveItems();

		await _subscriber.SendAsync(_map.From(item).To<NoteUpdated>());
	}

	public async Task DeleteItemAsync(Guid id)
	{
		var oldItem = _items.FirstOrDefault(arg => arg.Id == id);
		_items.Remove(oldItem);

		SaveItems();

		await _subscriber.SendAsync(new NoteDeleted(id));
	}

	public async Task<NoteModel> GetItemAsync(Guid id)
	{
		return await Task.FromResult(_items.FirstOrDefault(s => s.Id == id));
	}

	public Task<NoteListItemModel[]> GetItemsAsync()
	{
		_items = GetItems();

		var listItems = _map.From<NoteModel>(_items).ToArray<NoteListItemModel>();
		return Task.FromResult(listItems);
	}

	public Task<NoteListItemModel[]> GetItemsAsync(NoteFilter filter)
	{
		IEnumerable<NoteModel> enumerable = GetItems();

		if (!string.IsNullOrEmpty(filter.Title))
			enumerable = enumerable.Where(e => e.Title.ToLower().Contains(filter.Title.ToLower()));
		if (!string.IsNullOrEmpty(filter.Content))
			enumerable = enumerable.Where(e => e.Content.ToLower().Contains(filter.Content.ToLower()));
		if (filter.Color != null)
			enumerable = enumerable.Where(e => e.Color == filter.Color);

		var listItems = _map.From<NoteModel>(enumerable).ToArray<NoteListItemModel>();
		return Task.FromResult(listItems);
	}

	public void Dispose()
	{
		// Nothing to do
	}
}