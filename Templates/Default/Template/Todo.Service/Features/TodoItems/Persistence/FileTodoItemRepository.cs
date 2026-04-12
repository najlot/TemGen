using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Serialization;
using <# Project.Namespace#>.Service.Shared.Configuration;
using <# Project.Namespace#>.Service.Features.<# Definition.Name#>s;


namespace <# Project.Namespace#>.Service.Features.<# Definition.Name#>s.Persistence;

public class File<# Definition.Name#>Repository : I<# Definition.Name#>Repository
{
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
			var item = JsonSerializer.Deserialize<<# Definition.Name#>Model>(bytes, ServiceJsonSerializer.Options);
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
		var item = JsonSerializer.Deserialize<<# Definition.Name#>Model>(bytes, ServiceJsonSerializer.Options);

		return item;
	}

	public async Task Insert(<# Definition.Name#>Model model)
	{
		await Update(model).ConfigureAwait(false);
	}

	public async Task Update(<# Definition.Name#>Model model)
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
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>