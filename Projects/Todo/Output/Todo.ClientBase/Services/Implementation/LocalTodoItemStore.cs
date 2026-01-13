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

public sealed class LocalTodoItemStore : ITodoItemRepository
{
	private readonly string _dataPath;
	private readonly ILocalSubscriber _subscriber;
	private readonly IMap _map;
	private List<TodoItemModel> _items = null;

	public LocalTodoItemStore(string folderName, ILocalSubscriber localSubscriber, IMap map)
	{
		_map = map;

		var appdataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Todo");
		appdataDir = Path.Combine(appdataDir, folderName);
		Directory.CreateDirectory(appdataDir);

		_dataPath = Path.Combine(appdataDir, "TodoItems.json");
		_items = GetItems();
		_subscriber = localSubscriber;
	}

	private List<TodoItemModel> GetItems()
	{
		List<TodoItemModel> items;
		if (File.Exists(_dataPath))
		{
			var data = File.ReadAllText(_dataPath);
			items = JsonSerializer.Deserialize<List<TodoItemModel>>(data, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
		}
		else
		{
			items = new List<TodoItemModel>();
		}

		return items;
	}

	public async Task AddItemAsync(TodoItemModel item)
	{
		_items.Insert(0, item);

		SaveItems();

		await _subscriber.SendAsync(_map.From(item).To<TodoItemCreated>());
	}

	private void SaveItems()
	{
		var text = JsonSerializer.Serialize(_items);
		File.WriteAllText(_dataPath, text);
	}

	public async Task UpdateItemAsync(TodoItemModel item)
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

		await _subscriber.SendAsync(_map.From(item).To<TodoItemUpdated>());
	}

	public async Task DeleteItemAsync(Guid id)
	{
		var oldItem = _items.FirstOrDefault(arg => arg.Id == id);
		_items.Remove(oldItem);

		SaveItems();

		await _subscriber.SendAsync(new TodoItemDeleted(id));
	}

	public async Task<TodoItemModel> GetItemAsync(Guid id)
	{
		return await Task.FromResult(_items.FirstOrDefault(s => s.Id == id));
	}

	public Task<TodoItemListItemModel[]> GetItemsAsync()
	{
		_items = GetItems();

		var listItems = _map.From<TodoItemModel>(_items).ToArray<TodoItemListItemModel>();
		return Task.FromResult(listItems);
	}

	public Task<TodoItemListItemModel[]> GetItemsAsync(TodoItemFilter filter)
	{
		IEnumerable<TodoItemModel> enumerable = GetItems();

		if (!string.IsNullOrEmpty(filter.Title))
			enumerable = enumerable.Where(e => e.Title.ToLower().Contains(filter.Title.ToLower()));
		if (!string.IsNullOrEmpty(filter.Content))
			enumerable = enumerable.Where(e => e.Content.ToLower().Contains(filter.Content.ToLower()));
		if (filter.CreatedAtFrom != null)
			enumerable = enumerable.Where(e => e.CreatedAt >= filter.CreatedAtFrom);
		if (filter.CreatedAtTo != null)
			enumerable = enumerable.Where(e => e.CreatedAt <= filter.CreatedAtTo);
		if (!string.IsNullOrEmpty(filter.CreatedBy))
			enumerable = enumerable.Where(e => e.CreatedBy.ToLower().Contains(filter.CreatedBy.ToLower()));
		if (filter.AssignedToId != null)
			enumerable = enumerable.Where(e => e.AssignedToId == filter.AssignedToId);
		if (filter.Status != null)
			enumerable = enumerable.Where(e => e.Status == filter.Status);
		if (filter.ChangedAtFrom != null)
			enumerable = enumerable.Where(e => e.ChangedAt >= filter.ChangedAtFrom);
		if (filter.ChangedAtTo != null)
			enumerable = enumerable.Where(e => e.ChangedAt <= filter.ChangedAtTo);
		if (!string.IsNullOrEmpty(filter.ChangedBy))
			enumerable = enumerable.Where(e => e.ChangedBy.ToLower().Contains(filter.ChangedBy.ToLower()));
		if (!string.IsNullOrEmpty(filter.Priority))
			enumerable = enumerable.Where(e => e.Priority.ToLower().Contains(filter.Priority.ToLower()));

		var listItems = _map.From<TodoItemModel>(enumerable).ToArray<TodoItemListItemModel>();
		return Task.FromResult(listItems);
	}

	public void Dispose()
	{
		// Nothing to do
	}
}