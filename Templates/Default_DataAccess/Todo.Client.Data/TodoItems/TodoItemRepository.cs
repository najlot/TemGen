using Najlot.Map;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.Data;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Contracts.<# Definition.Name#>s;

namespace <# Project.Namespace#>.Client.Data.<# Definition.Name#>s;

public class <# Definition.Name#>Repository(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider, IMap map)
	: HttpClientRepository(httpClientFactory, tokenProvider), I<# Definition.Name#>Repository
{
	public async Task<<# Definition.Name#>ListItemModel[]> GetItemsAsync()
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var items = await client.GetFromJsonAsync<<# Definition.Name#>ListItem[]>("api/<# Definition.Name#>", ClientDataJsonSerializer.Options).ConfigureAwait(false) ?? [];
		return map.From<<# Definition.Name#>ListItem>(items).ToArray<<# Definition.Name#>ListItemModel>();
	}

	public async Task<<# Definition.Name#>ListItemModel[]> GetItemsAsync(EntityFilter filter)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var response = await client.PostAsJsonAsync("api/<# Definition.Name#>/ListFiltered", filter, ClientDataJsonSerializer.Options).ConfigureAwait(false);
		var items = await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<<# Definition.Name#>ListItem[]>(ClientDataJsonSerializer.Options).ConfigureAwait(false) ?? [];
		return map.From<<# Definition.Name#>ListItem>(items).ToArray<<# Definition.Name#>ListItemModel>();
	}

	public async Task<<# Definition.Name#>Model> GetItemAsync(Guid id)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var item = await client.GetFromJsonAsync<<# Definition.Name#>>($"api/<# Definition.Name#>/{id}", ClientDataJsonSerializer.Options).ConfigureAwait(false);
		return map.From(item).To<<# Definition.Name#>Model>();
	}

	public async Task AddItemAsync(<# Definition.Name#>Model item)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var request = map.From(item).To<Create<# Definition.Name#>>();
		var response = await client.PostAsJsonAsync("api/<# Definition.Name#>", request, ClientDataJsonSerializer.Options).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task UpdateItemAsync(<# Definition.Name#>Model item)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var request = map.From(item).To<Update<# Definition.Name#>>();
		var response = await client.PutAsJsonAsync("api/<# Definition.Name#>", request, ClientDataJsonSerializer.Options).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task DeleteItemAsync(Guid id)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var response = await client.DeleteAsync($"api/<# Definition.Name#>/{id}").ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>