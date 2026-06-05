using Najlot.Map;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.Client.Data.Filters;

public sealed class FilterRepository(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider, IMap map)
	: HttpClientRepository(httpClientFactory, tokenProvider), IFilterRepository
{
	public async Task<Filter[]> GetItemsAsync(ItemType targetType)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		return await client.GetFromJsonAsync<Filter[]>(CreateRequestUri(targetType), ClientDataJsonSerializer.Options).ConfigureAwait(false) ?? [];
	}

	public async Task AddItemAsync(Filter item)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var request = map.From(item).To<CreateFilter>();
		var response = await client.PostAsJsonAsync("api/Filters", request, ClientDataJsonSerializer.Options).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task UpdateItemAsync(Filter item)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var request = map.From(item).To<UpdateFilter>();
		var response = await client.PutAsJsonAsync("api/Filters", request, ClientDataJsonSerializer.Options).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task DeleteItemAsync(Guid id)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var response = await client.DeleteAsync($"api/Filters/{id}").ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	private static string CreateRequestUri(ItemType targetType)
		=> $"api/Filters?targetType={Uri.EscapeDataString(targetType.ToString())}";
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>