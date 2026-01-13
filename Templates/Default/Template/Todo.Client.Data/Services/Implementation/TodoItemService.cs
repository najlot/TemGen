using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Client.Data.Repositories;
using <#cs Write(Project.Namespace)#>.Contracts.Filters;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Services.Implementation;

public sealed class <#cs Write(Definition.Name)#>Service : I<#cs Write(Definition.Name)#>Service
{
	private readonly I<#cs Write(Definition.Name)#>Repository _repository;

	public <#cs Write(Definition.Name)#>Service(I<#cs Write(Definition.Name)#>Repository repository)
	{
		_repository = repository;
	}

	public <#cs Write(Definition.Name)#>Model Create<#cs Write(Definition.Name)#>()
	{
		return new <#cs Write(Definition.Name)#>Model()
		{
			Id = Guid.NewGuid(),
<#cs
foreach(var entry in Entries)
{
	if (entry.IsOwnedType)
	{
		WriteLine($"			{entry.Field} = new (),");
	}
	else if(entry.EntryType.ToLower() == "string")
	{
		WriteLine($"			{entry.Field} = \"\",");
	}
	else if(entry.IsArray)
	{
		WriteLine($"			{entry.Field} = [],");
	}
	
}

Result = Result.TrimEnd(' ', '\r', '\n', ',');
#>
		};
	}

	public async Task AddItemAsync(<#cs Write(Definition.Name)#>Model item)
	{
		await _repository.AddItemAsync(item);
	}

	public async Task DeleteItemAsync(Guid id)
	{
		await _repository.DeleteItemAsync(id);
	}

	public async Task<<#cs Write(Definition.Name)#>Model> GetItemAsync(Guid id)
	{
		return await _repository.GetItemAsync(id);
	}

	public async Task<IEnumerable<<#cs Write(Definition.Name)#>ListItemModel>> GetItemsAsync()
	{
		return await _repository.GetItemsAsync();
	}

	public async Task<IEnumerable<<#cs Write(Definition.Name)#>ListItemModel>> GetItemsAsync(<#cs Write(Definition.Name)#>Filter filter)
	{
		return await _repository.GetItemsAsync(filter);
	}

	public async Task UpdateItemAsync(<#cs Write(Definition.Name)#>Model item)
	{
		await _repository.UpdateItemAsync(item);
	}

	public void Dispose() => _repository.Dispose();
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>