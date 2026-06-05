using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.Data;
using <# Project.Namespace#>.Contracts.History;

namespace <# Project.Namespace#>.Client.Data.History;

public class HistoryRepository(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
	: HttpClientRepository(httpClientFactory, tokenProvider), IHistoryRepository
{
	public async Task<HistoryEntry[]> GetItemsAsync(Guid entityId)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		return await client.GetFromJsonAsync<HistoryEntry[]>($"api/History/{entityId}", ClientDataJsonSerializer.Options).ConfigureAwait(false) ?? [];
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>