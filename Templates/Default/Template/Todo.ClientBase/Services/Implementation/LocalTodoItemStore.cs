using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Najlot.Map;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Client.Data.Repositories;
using <#cs Write(Project.Namespace)#>.ClientBase.ProfileHandler;
using <#cs Write(Project.Namespace)#>.Contracts;
using <#cs Write(Project.Namespace)#>.Contracts.Events;
using <#cs Write(Project.Namespace)#>.Contracts.Filters;

namespace <#cs Write(Project.Namespace)#>.ClientBase.Services.Implementation;

public sealed class Local<#cs Write(Definition.Name)#>Store : I<#cs Write(Definition.Name)#>Repository
{
	private readonly string _dataPath;
	private readonly ILocalSubscriber _subscriber;
	private readonly IMap _map;
	private List<<#cs Write(Definition.Name)#>Model> _items = null;

	public Local<#cs Write(Definition.Name)#>Store(string folderName, ILocalSubscriber localSubscriber, IMap map)
	{
		_map = map;

		var appdataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "<#cs Write(Project.Namespace)#>");
		appdataDir = Path.Combine(appdataDir, folderName);
		Directory.CreateDirectory(appdataDir);

		_dataPath = Path.Combine(appdataDir, "<#cs Write(Definition.Name)#>s.json");
		_items = GetItems();
		_subscriber = localSubscriber;
	}

	private List<<#cs Write(Definition.Name)#>Model> GetItems()
	{
		List<<#cs Write(Definition.Name)#>Model> items;
		if (File.Exists(_dataPath))
		{
			var data = File.ReadAllText(_dataPath);
			items = JsonSerializer.Deserialize<List<<#cs Write(Definition.Name)#>Model>>(data, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
		}
		else
		{
			items = new List<<#cs Write(Definition.Name)#>Model>();
		}

		return items;
	}

	public async Task AddItemAsync(<#cs Write(Definition.Name)#>Model item)
	{
		_items.Insert(0, item);

		SaveItems();

		await _subscriber.SendAsync(_map.From(item).To<<#cs Write(Definition.Name)#>Created>());
	}

	private void SaveItems()
	{
		var text = JsonSerializer.Serialize(_items);
		File.WriteAllText(_dataPath, text);
	}

	public async Task UpdateItemAsync(<#cs Write(Definition.Name)#>Model item)
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

		await _subscriber.SendAsync(_map.From(item).To<<#cs Write(Definition.Name)#>Updated>());
	}

	public async Task DeleteItemAsync(Guid id)
	{
		var oldItem = _items.FirstOrDefault(arg => arg.Id == id);
		_items.Remove(oldItem);

		SaveItems();

		await _subscriber.SendAsync(new <#cs Write(Definition.Name)#>Deleted(id));
	}

	public async Task<<#cs Write(Definition.Name)#>Model> GetItemAsync(Guid id)
	{
		return await Task.FromResult(_items.FirstOrDefault(s => s.Id == id));
	}

	public Task<<#cs Write(Definition.Name)#>ListItemModel[]> GetItemsAsync()
	{
		_items = GetItems();

		var listItems = _map.From<<#cs Write(Definition.Name)#>Model>(_items).ToArray<<#cs Write(Definition.Name)#>ListItemModel>();
		return Task.FromResult(listItems);
	}

	public Task<<#cs Write(Definition.Name)#>ListItemModel[]> GetItemsAsync(<#cs Write(Definition.Name)#>Filter filter)
	{
		IEnumerable<<#cs Write(Definition.Name)#>Model> enumerable = GetItems();

<#cs
foreach(var entry in Entries)
{
    if (entry.IsReference)
    {
        WriteLine($"		if (filter.{entry.Field}Id != null)");
		WriteLine($"			enumerable = enumerable.Where(e => e.{entry.Field}Id == filter.{entry.Field}Id);");
    }
    else if (entry.EntryType == "long"
        || entry.EntryType == "short"
        || entry.EntryType == "int"
        || entry.EntryType == "ulong"
        || entry.EntryType == "ushort"
        || entry.EntryType == "uint"
        || entry.EntryType == "DateTime"
        )
    {
        WriteLine($"		if (filter.{entry.Field}From != null)");
		WriteLine($"			enumerable = enumerable.Where(e => e.{entry.Field} >= filter.{entry.Field}From);");

		WriteLine($"		if (filter.{entry.Field}To != null)");
		WriteLine($"			enumerable = enumerable.Where(e => e.{entry.Field} <= filter.{entry.Field}To);");
    }
	else if (entry.EntryType.ToLower() == "string")
    {
        WriteLine($"		if (!string.IsNullOrEmpty(filter.{entry.Field}))");
		WriteLine($"			enumerable = enumerable.Where(e => e.{entry.Field}.ToLower().Contains(filter.{entry.Field}.ToLower()));");
    }
    else if (!(entry.IsArray || entry.IsOwnedType))
    {
        WriteLine($"		if (filter.{entry.Field} != null)");
		WriteLine($"			enumerable = enumerable.Where(e => e.{entry.Field} == filter.{entry.Field});");
    }
}

Result = Result.TrimEnd();
#>

		var listItems = _map.From<<#cs Write(Definition.Name)#>Model>(enumerable).ToArray<<#cs Write(Definition.Name)#>ListItemModel>();
		return Task.FromResult(listItems);
	}

	public void Dispose()
	{
		// Nothing to do
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>