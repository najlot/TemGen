using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Contracts.Favorites;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.Client.Data.Favorites;

public sealed class FavoriteRepository(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
	: HttpClientRepository(httpClientFactory, tokenProvider), IFavoriteRepository
{
	public async Task<Favorite[]> GetItemsAsync(ItemType targetType)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		return await client.GetFromJsonAsync<Favorite[]>(CreateRequestUri(targetType), ClientDataJsonSerializer.Options).ConfigureAwait(false) ?? [];
	}

	public async Task AddItemAsync(ItemType targetType, Guid itemId)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var request = new CreateFavorite
		{
			TargetType = targetType,
			ItemId = itemId,
		};
		var response = await client.PostAsJsonAsync("api/Favorites", request, ClientDataJsonSerializer.Options).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task DeleteItemAsync(ItemType targetType, Guid itemId)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var response = await client.DeleteAsync($"api/Favorites/{targetType}/{itemId}").ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	private static string CreateRequestUri(ItemType targetType)
		=> $"api/Favorites?targetType={Uri.EscapeDataString(targetType.ToString())}";
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>