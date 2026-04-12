using Najlot.Map;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Todo.Client.Data.Identity;
using Todo.Client.Data.Serialization;
using Todo.Contracts.GlobalSearch;

namespace Todo.Client.Data.GlobalSearch;

public sealed class GlobalSearchRepository(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider, IMap map)
	: HttpClientRepository(httpClientFactory, tokenProvider), IGlobalSearchRepository
{
	public async Task<GlobalSearchItemModel[]> SearchAsync(string text)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var encodedText = Uri.EscapeDataString(text);
		var items = await client.GetFromJsonAsync<GlobalSearchItem[]>($"api/GlobalSearch?text={encodedText}", ClientDataJsonSerializer.Options).ConfigureAwait(false) ?? [];
		return map.From<GlobalSearchItem>(items).ToArray<GlobalSearchItemModel>();
	}
}
