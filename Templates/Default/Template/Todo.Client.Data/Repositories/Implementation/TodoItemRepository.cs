using Cosei.Client.Base;
using Najlot.Map;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Client.Data.Identity;
using <#cs Write(Project.Namespace)#>.Contracts;
using <#cs Write(Project.Namespace)#>.Contracts.Commands;
using <#cs Write(Project.Namespace)#>.Contracts.Filters;
using <#cs Write(Project.Namespace)#>.Contracts.ListItems;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Repositories.Implementation;

public class <#cs Write(Definition.Name)#>Repository : I<#cs Write(Definition.Name)#>Repository
{
	private readonly IRequestClient _client;
	private readonly ITokenProvider _tokenProvider;
	private readonly IMap _map;

	public <#cs Write(Definition.Name)#>Repository(IRequestClient client, ITokenProvider tokenProvider, IMap map)
	{
		_tokenProvider = tokenProvider;
		_client = client;
		_map = map;
	}

	public async Task<<#cs Write(Definition.Name)#>ListItemModel[]> GetItemsAsync()
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var items = await _client.GetAsync<<#cs Write(Definition.Name)#>ListItem[]>("api/<#cs Write(Definition.Name)#>", headers);
		return _map.From<<#cs Write(Definition.Name)#>ListItem>(items).ToArray<<#cs Write(Definition.Name)#>ListItemModel>();
	}

	public async Task<<#cs Write(Definition.Name)#>ListItemModel[]> GetItemsAsync(<#cs Write(Definition.Name)#>Filter filter)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var items = await _client.PostAsync<List<<#cs Write(Definition.Name)#>ListItem>, <#cs Write(Definition.Name)#>Filter>("api/<#cs Write(Definition.Name)#>/ListFiltered", filter, headers);
		return _map.From<<#cs Write(Definition.Name)#>ListItem>(items).ToArray<<#cs Write(Definition.Name)#>ListItemModel>();
	}

	public async Task<<#cs Write(Definition.Name)#>Model> GetItemAsync(Guid id)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var item = await _client.GetAsync<<#cs Write(Definition.Name)#>>($"api/<#cs Write(Definition.Name)#>/{id}", headers);
		return _map.From(item).To<<#cs Write(Definition.Name)#>Model>();
	}

	public async Task AddItemAsync(<#cs Write(Definition.Name)#>Model item)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var request = _map.From(item).To<Create<#cs Write(Definition.Name)#>>();
		await _client.PostAsync("api/<#cs Write(Definition.Name)#>", request, headers);
	}

	public async Task UpdateItemAsync(<#cs Write(Definition.Name)#>Model item)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var request = _map.From(item).To<Update<#cs Write(Definition.Name)#>>();
		await _client.PutAsync("api/<#cs Write(Definition.Name)#>", request, headers);
	}

	public async Task DeleteItemAsync(Guid id)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var response = await _client.DeleteAsync($"api/<#cs Write(Definition.Name)#>/{id}", headers);
		response.EnsureSuccessStatusCode();
	}

	public void Dispose() => _client.Dispose();
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>