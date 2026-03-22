using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Configuration;
using <# Project.Namespace#>.Service.Model;
using <# Project.Namespace#>.Service.Repository;

namespace <# Project.Namespace#>.Service.Repository.FileImpl;

public class File<# Definition.Name#>Repository : I<# Definition.Name#>Repository
{
	private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
	private readonly string _storagePath;

	public File<# Definition.Name#>Repository(FileConfiguration configuration)
	{
		_storagePath = configuration.<# Definition.Name#>sPath;
		Directory.CreateDirectory(_storagePath);
	}

	public IQueryable<<# Definition.Name#>Model> GetAllQueryable()
	{
		var items = new List<<# Definition.Name#>Model>();

		foreach (var path in Directory.GetFiles(_storagePath))
		{
			var bytes = File.ReadAllBytes(path);
			var text = Encoding.UTF8.GetString(bytes);
			var item = JsonSerializer.Deserialize<<# Definition.Name#>Model>(text, _options);
			if (item is not null)
			{
				items.Add(item);
			}
		}

		return items.AsQueryable();
	}

	public async Task<<# Definition.Name#>Model?> Get(Guid id)
	{
		var path = Path.Combine(_storagePath, id.ToString());

		if (!File.Exists(path))
		{
			return null;
		}

		var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
		var item = JsonSerializer.Deserialize<<# Definition.Name#>Model>(bytes, _options);

		return item;
	}

	public async Task Insert(<# Definition.Name#>Model model)
	{
		await Update(model).ConfigureAwait(false);
	}

	public async Task Update(<# Definition.Name#>Model model)
	{
		var path = Path.Combine(_storagePath, model.Id.ToString());
		var bytes = JsonSerializer.SerializeToUtf8Bytes(model);
		await File.WriteAllBytesAsync(path, bytes).ConfigureAwait(false);
	}

	public Task Delete(Guid id)
	{
		var path = Path.Combine(_storagePath, id.ToString());
		File.Delete(path);
		return Task.CompletedTask;
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>