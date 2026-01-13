using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Service.Configuration;
using <#cs Write(Project.Namespace)#>.Service.Model;

namespace <#cs Write(Project.Namespace)#>.Service.Repository;

public class File<#cs Write(Definition.Name)#>Repository : I<#cs Write(Definition.Name)#>Repository
{
	private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
	private readonly string _storagePath;

	public File<#cs Write(Definition.Name)#>Repository(FileConfiguration configuration)
	{
		_storagePath = configuration.<#cs Write(Definition.Name)#>sPath;
		Directory.CreateDirectory(_storagePath);
	}

	public async IAsyncEnumerable<<#cs Write(Definition.Name)#>Model> GetAll()
	{
		foreach (var path in Directory.GetFiles(_storagePath))
		{
			var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
			var text = Encoding.UTF8.GetString(bytes);
			var item = JsonSerializer.Deserialize<<#cs Write(Definition.Name)#>Model>(text, _options);
			if (item is not null)
			{
				yield return item;
			}
		}
	}

	public IQueryable<<#cs Write(Definition.Name)#>Model> GetAllQueryable()
	{
		var items = new List<<#cs Write(Definition.Name)#>Model>();

		foreach (var path in Directory.GetFiles(_storagePath))
		{
			var bytes = File.ReadAllBytes(path);
			var text = Encoding.UTF8.GetString(bytes);
			var item = JsonSerializer.Deserialize<<#cs Write(Definition.Name)#>Model>(text, _options);
			if (item is not null)
			{
				items.Add(item);
			}
		}

		return items.AsQueryable();
	}

	public async Task<<#cs Write(Definition.Name)#>Model?> Get(Guid id)
	{
		var path = Path.Combine(_storagePath, id.ToString());

		if (!File.Exists(path))
		{
			return null;
		}

		var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
		var item = JsonSerializer.Deserialize<<#cs Write(Definition.Name)#>Model>(bytes, _options);

		return item;
	}

	public async Task Insert(<#cs Write(Definition.Name)#>Model model)
	{
		await Update(model).ConfigureAwait(false);
	}

	public async Task Update(<#cs Write(Definition.Name)#>Model model)
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