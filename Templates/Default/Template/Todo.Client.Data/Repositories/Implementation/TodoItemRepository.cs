using Najlot.Map;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Contracts.Commands;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Contracts.ListItems;

namespace <# Project.Namespace#>.Client.Data.Repositories.Implementation;

public class <# Definition.Name#>Repository(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider, IMap map)
	: HttpClientRepository(httpClientFactory, tokenProvider), I<# Definition.Name#>Repository
{
	public async Task<<# Definition.Name#>ListItemModel[]> GetItemsAsync()
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var items = await client.GetFromJsonAsync<<# Definition.Name#>ListItem[]>("api/<# Definition.Name#>").ConfigureAwait(false) ?? [];
		return map.From<<# Definition.Name#>ListItem>(items).ToArray<<# Definition.Name#>ListItemModel>();
	}

	public async Task<<# Definition.Name#>ListItemModel[]> GetItemsAsync(<# Definition.Name#>Filter filter)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var response = await client.PostAsJsonAsync("api/<# Definition.Name#>/ListFiltered", filter).ConfigureAwait(false);
		var items = await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<<# Definition.Name#>ListItem[]>().ConfigureAwait(false) ?? [];
		return map.From<<# Definition.Name#>ListItem>(items).ToArray<<# Definition.Name#>ListItemModel>();
	}

	public async Task<<# Definition.Name#>Model> GetItemAsync(Guid id)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var item = await client.GetFromJsonAsync<<# Definition.Name#>>($"api/<# Definition.Name#>/{id}").ConfigureAwait(false);
		return map.From(item).To<<# Definition.Name#>Model>();
	}

	public async Task AddItemAsync(<# Definition.Name#>Model item)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var request = map.From(item).To<Create<# Definition.Name#>>();
		var response = await client.PostAsJsonAsync("api/<# Definition.Name#>", request).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task UpdateItemAsync(<# Definition.Name#>Model item)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var request = map.From(item).To<Update<# Definition.Name#>>();
		var response = await client.PutAsJsonAsync("api/<# Definition.Name#>", request).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task DeleteItemAsync(Guid id)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var response = await client.DeleteAsync($"api/<# Definition.Name#>/{id}").ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>