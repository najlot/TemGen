using Najlot.Map;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Client.Data.Identity;
using <#cs Write(Project.Namespace)#>.Contracts;
using <#cs Write(Project.Namespace)#>.Contracts.Commands;
using <#cs Write(Project.Namespace)#>.Contracts.Filters;
using <#cs Write(Project.Namespace)#>.Contracts.ListItems;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Repositories.Implementation;

public class <#cs Write(Definition.Name)#>Repository(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider, IMap map)
	: HttpClientRepository(httpClientFactory, tokenProvider), I<#cs Write(Definition.Name)#>Repository
{
	public async Task<<#cs Write(Definition.Name)#>ListItemModel[]> GetItemsAsync()
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var items = await client.GetFromJsonAsync<<#cs Write(Definition.Name)#>ListItem[]>("api/<#cs Write(Definition.Name)#>").ConfigureAwait(false) ?? [];
		return map.From<<#cs Write(Definition.Name)#>ListItem>(items).ToArray<<#cs Write(Definition.Name)#>ListItemModel>();
	}

	public async Task<<#cs Write(Definition.Name)#>ListItemModel[]> GetItemsAsync(<#cs Write(Definition.Name)#>Filter filter)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var response = await client.PostAsJsonAsync("api/<#cs Write(Definition.Name)#>/ListFiltered", filter).ConfigureAwait(false);
		var items = await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<<#cs Write(Definition.Name)#>ListItem[]>().ConfigureAwait(false) ?? [];
		return map.From<<#cs Write(Definition.Name)#>ListItem>(items).ToArray<<#cs Write(Definition.Name)#>ListItemModel>();
	}

	public async Task<<#cs Write(Definition.Name)#>Model> GetItemAsync(Guid id)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var item = await client.GetFromJsonAsync<<#cs Write(Definition.Name)#>>($"api/<#cs Write(Definition.Name)#>/{id}").ConfigureAwait(false);
		return map.From(item).To<<#cs Write(Definition.Name)#>Model>();
	}

	public async Task AddItemAsync(<#cs Write(Definition.Name)#>Model item)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var request = map.From(item).To<Create<#cs Write(Definition.Name)#>>();
		var response = await client.PostAsJsonAsync("api/<#cs Write(Definition.Name)#>", request).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task UpdateItemAsync(<#cs Write(Definition.Name)#>Model item)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var request = map.From(item).To<Update<#cs Write(Definition.Name)#>>();
		var response = await client.PutAsJsonAsync("api/<#cs Write(Definition.Name)#>", request).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task DeleteItemAsync(Guid id)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var response = await client.DeleteAsync($"api/<#cs Write(Definition.Name)#>/{id}").ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>