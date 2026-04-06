using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Client.Data.Repositories.Implementation;

public class HistoryRepository(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
	: HttpClientRepository(httpClientFactory, tokenProvider), IHistoryRepository
{
	public async Task<HistoryEntry[]> GetItemsAsync(Guid entityId)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		return await client.GetFromJsonAsync<HistoryEntry[]>($"api/History/{entityId}").ConfigureAwait(false) ?? [];
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>